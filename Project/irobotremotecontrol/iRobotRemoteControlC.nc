#include "AM.h"
#include "Serial.h"
#include "oi.h"  ///this is oi.h file from iRobot Create demo files.
#include "IRobot.h"

module iRobotRemoteControlC {
	uses {
		interface Boot;
		interface Leds;
		interface Timer<TMilli> as Timer0; //this is being used like a signal to show that everything is fine 
		interface SplitControl as RadioControl;
		interface SplitControl as SerialControl; //to start and stop serial section of system		
		interface UartByte; //for sending and receiving one byte at a time -- no interrupts here
		interface UartStream; //multiple byte send and receive, byte level receive interrupt                                                                           

		interface Packet;
		interface AMPacket;
		interface AMSend as RadioSend;
		interface Receive as RadioReceive;
		
		//Add for sending rssi
		interface Timer<TMilli> as SendTimer;
		interface AMSend as RssiMsgSend;

	}
}
implementation {
  
	//For sending rssi
	 message_t empty_msg;

	message_t radioQueueBufs[RADIO_QUEUE_LEN];
	message_t * ONE_NOK radioQueue[RADIO_QUEUE_LEN];
	uint8_t radioIn, radioOut;
	bool radioBusy, radioFull;

	//TOS_NODE_ID = 0; //means this is Gateway
	uint8_t cmd;
	message_t pkt;
	bool radioBusy = FALSE;
	bool serialBusy = FALSE;
	uint8_t receivedByte; //from serial port    
	uint8_t selectedRobot = 1;
	bool robotSelectMode = FALSE;
	bool isThisGateway = TRUE;
	uint8_t destinationAddress = 0; //I assume 0 as gateway  --default destination 

	task void SendToRadio();
	void Fail(uint8_t code);
	void OK();

	event void Boot.booted() {
		uint8_t i;

		//Radio queue, filled by UART and consumed by Radio
		for(i = 0; i < RADIO_QUEUE_LEN; i++) 
			radioQueue[i] = &radioQueueBufs[i];
		radioIn = radioOut = 0;
		radioBusy = FALSE;
		radioFull = TRUE;

		//set this true for Gateway    !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!		
		atomic isThisGateway = TRUE;
		if(isThisGateway) 
			destinationAddress = selectedRobot;

		call RadioControl.start();
	}

	event void RadioControl.startDone(error_t error) {
		if(error == SUCCESS) {
			radioFull = FALSE;
			call SerialControl.start();
		}
		else {
			Fail(1);
			call RadioControl.start();//try again						
		}
	}

	event void RadioControl.stopDone(error_t error) {
	}

	event void SerialControl.startDone(error_t error) {
		if(error == SUCCESS) {
			if(call UartStream.enableReceiveInterrupt() != SUCCESS) {
				Fail(1);
			}
			else {
				call Timer0.startPeriodic(TIMER_INTERVAL);
				//For sending rssi
				call SendTimer.startPeriodic(SEND_INTERVAL_MS);
			}

		}
		else {
			Fail(1);
			call SerialControl.start(); //try again		
		}

	}
	event void SerialControl.stopDone(error_t error) {
	}
	
	//For sending rssi
	  event void SendTimer.fired(){
	    call RssiMsgSend.send(AM_BROADCAST_ADDR, &empty_msg, sizeof(RssiMsg));    
	  }

	  event void RssiMsgSend.sendDone(message_t *m, error_t error){}

	////////*************************************************************************uart to radio section                        

	async event void UartStream.receivedByte(uint8_t byte) {

		atomic if( ! radioFull) {
			iRobotMsg * btrpkt = (iRobotMsg * )(call Packet
					.getPayload(radioQueue[radioIn], sizeof(iRobotMsg)));

			btrpkt->nodeid = TOS_NODE_ID;
			btrpkt->cmd = byte;

			if(++radioIn >= RADIO_QUEUE_LEN) 
				radioIn = 0;
			if(radioIn == radioOut) 
				radioFull = TRUE;

			if( ! radioBusy) {
				post SendToRadio();
				radioBusy = TRUE;
			}
		}
		else 
			Fail(2);
	}

	void task SendToRadio() {

		atomic if(radioIn == radioOut && ! radioFull) {
			radioBusy = FALSE;
			return;
		}
		
		if(call RadioSend.send(destinationAddress, radioQueue[radioOut],
				sizeof(iRobotMsg)) == SUCCESS) 
			OK();
		else {
			Fail(2);
			post SendToRadio();
		}
	}

	event void RadioSend.sendDone(message_t * msg, error_t error) {
		if(error != SUCCESS) 
			Fail(2);
		else 
			atomic if(msg == radioQueue[radioOut])
			//I think I can remove this since I only have one place to send to radio and this always will be from same source.
		{
			if(++radioOut >= RADIO_QUEUE_LEN) 
				radioOut = 0;
			if(radioFull) 
				radioFull = FALSE;
		}

		post SendToRadio();

	}

	///***************************************************************end of radio to uart section                   

	//*******************************************************Radio to Uart Section     

	//this will be triggered only and only if the address of the packet is my address or it is broadcast. 
	//if we need to handle other packets, we should use snoop receive 
	event message_t * RadioReceive.receive(message_t * msg, void * payload,
			uint8_t len) {
			uint8_t commandid = 0;
		atomic {

				if(len == sizeof(iRobotMsg)){//this will be correct always since we only have one kind of packets so far
					iRobotMsg * btrpkt = (iRobotMsg * ) payload;
					commandid = btrpkt->cmd;
					
					if(TOS_NODE_ID == 1) {						
						switch(commandid)
						{
							case 200:
								while(call UartStream.send(SING_COMMAND,15) != SUCCESS);
							break;
							case 201:
								while(call UartStream.send(BLINK_COMMAND,6) != SUCCESS);
							break;
							case 202:
								while(call UartStream.send(CLIFF_COMMAND,7) != SUCCESS);
							break;
							case 203:
								while(call UartStream.send(FORWARD_COMMAND,7) != SUCCESS);
							break;
							case 204:
								while(call UartStream.send(BACK_COMMAND,7) != SUCCESS);
							break;
							case 205:
								while(call UartStream.send(LEFT_COMMAND,7) != SUCCESS);
							break;
							case 206:
								while(call UartStream.send(RIGHT_COMMAND,7) != SUCCESS);
							break;
							case 207:
								while(call UartStream.send(STOP_COMMAND,7) != SUCCESS);
							break;
							default:
							break;
						}
					}
					else if (TOS_NODE_ID == 0) {
						while(call UartStream.send(&commandid,1) != SUCCESS);
					}
				}
				else if(TOS_NODE_ID == 0)
				{
				  //I'm the base station, forward data onto serial
				  while(call UartStream.send(msg,call Packet.payloadLength(msg)) != SUCCESS);
				  
				}
		}

		return msg;
	}


	async event void UartStream.sendDone(uint8_t * buf, uint16_t len,
			error_t error) {
	}

	//*************************************************************end of Radio to UART section                

	async event void UartStream.receiveDone(uint8_t * buf, uint16_t len,
			error_t error) {
	}

	event void Timer0.fired() {
		OK();
	}

	void Fail(uint8_t code) {
		uint8_t leds = call Leds.get();
		call Leds.set(leds & 3); //turn off leds 0 and 1 , don''t touch led 2
		leds = call Leds.get();
		call Leds.set(code | leds);
	}

	void OK() {
		call Leds.led2Toggle();
	}

}



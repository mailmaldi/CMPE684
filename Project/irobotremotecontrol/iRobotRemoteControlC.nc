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
		interface Receive as RssiRadioReceive;
		#ifdef __CC2420_H__
		  interface CC2420Packet;
		#elif defined(TDA5250_MESSAGE_H)
		  interface Tda5250Packet;    
		#else
		  interface PacketField<uint8_t> as PacketRSSI;
		#endif 
		  
		interface Timer<TMilli> as RssiArraySendTimer;
		interface AMSend as RssiArraySend;
		interface Receive as RssiArrayReceive;
		
		interface AMSend as UartSend[am_id_t id];
		//interface Receive as UartReceive[am_id_t id];
		interface Packet as UartPacket;
		interface AMPacket as UartAMPacket;

	}
}
implementation {
  
	//For sending rssi
	 message_t empty_msg;
	 message_t empty_msg2;
	 uint16_t rssi[5] = {0,0,0,0,0};
	 bool startRssiArrayTimer = FALSE;

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
	bool isThisGateway = TRUE;
	uint8_t destinationAddress = 0; //I assume 0 as gateway  --default destination 

	task void SendToRadio();
	void Fail(uint8_t code);
	void OK();
	void Start_Timers();
	uint16_t getRssi(message_t *msg);
	
	event void Boot.booted() {
		uint8_t i;

		//Radio queue, filled by UART and consumed by Radio
		for(i = 0; i < RADIO_QUEUE_LEN; i++) 
			radioQueue[i] = &radioQueueBufs[i];
		radioIn = radioOut = 0;
		radioBusy = FALSE;
		radioFull = TRUE;

		//set this true for Gateway    !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		if(TOS_NODE_ID != 0)
		{
		  atomic isThisGateway = FALSE;
		}
		else if(TOS_NODE_ID == 0)
		{
		  atomic isThisGateway = TRUE;
		}
		
		if(isThisGateway) 
			destinationAddress = selectedRobot;

		call RadioControl.start();
	}

	event void RadioControl.startDone(error_t error) {
		if(error == SUCCESS) {
			radioFull = FALSE;
			//TODO do not start serial control for nodeid > 1, start blinking here itself
			if(TOS_NODE_ID > 1)
			{
			    Start_Timers();
			}
			else {
			    call SerialControl.start();
			}
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
				Start_Timers();
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
	    RssiMsg *rssiMsg;
	    rssiMsg = (RssiMsg*) (call Packet.getPayload(&empty_msg, sizeof (RssiMsg)));
	    rssiMsg->nodeid = TOS_NODE_ID;
	    while(call RssiMsgSend.send(AM_BROADCAST_ADDR, &empty_msg, sizeof(RssiMsg)) != SUCCESS );    //AM_BROADCAST_ADDR
	    
	  }

	  event void RssiMsgSend.sendDone(message_t *m, error_t error)
	  {
	    call SendTimer.startOneShot(SEND_INTERVAL_MS);
	  }
	  
	  
	  event void RssiArraySendTimer.fired() {
	    //message_t msg;
	    RssiArray * rssiArray;
	    int i = 0;
	    am_id_t id;
	    am_addr_t addr, src;
	    uint8_t len;
	    
	    rssiArray = (RssiArray*) (call Packet.getPayload(&empty_msg2, sizeof (RssiArray)));
	    rssiArray->nodeid = TOS_NODE_ID;
	    for(i=0;i<5;i++)
	    {
	      rssiArray->rssi[i] = rssi[i];
	    }
	    
	    call Packet.setPayloadLength(&empty_msg2, sizeof (RssiArray));
            call AMPacket.setDestination(&empty_msg2, 0);
            call AMPacket.setSource(&empty_msg2, TOS_NODE_ID);
	    
	    // MILIND: IF BASE STATION, then no need to send packet, but instead just send it over serial
	    if(TOS_NODE_ID == 0)
	    {
	      //TODO FIX THIS!!!
	      //while(call UartStream.send((void *) rssiArray, sizeof(RssiArray)) != SUCCESS)
	      id = call AMPacket.type(&empty_msg2);
	      addr = call AMPacket.destination(&empty_msg2);
	      len = call Packet.payloadLength(&empty_msg2);
	      src = call AMPacket.source(&empty_msg2);
	      call UartAMPacket.setSource(&empty_msg2, src);
	      startRssiArrayTimer = TRUE;
	      while(call UartSend.send[AM_RSSIARR](addr, &empty_msg2, len) != SUCCESS);
	      //TODO maybe run timer oneshot after serial xfer is done
	      return;
	    }
	    else
	    {
		while(call RssiArraySend.send(0, &empty_msg2, sizeof(RssiArray)) != SUCCESS );	    
	    } 
	     
	     call Leds.led1Toggle();	    
	  }
	  
	   event void RssiArraySend.sendDone(message_t *m, error_t error)
	   {
	      call RssiArraySendTimer.startOneShot(RSSI_ARRAY_INTERVAL_MS);
	   }

	////////*************************************************************************uart to radio section                        

	async event void UartStream.receivedByte(uint8_t byte) {

		atomic if( ! radioFull) {
			iRobotMsg * btrpkt = (iRobotMsg * )(call Packet
					.getPayload(radioQueue[radioIn], sizeof(iRobotMsg)));

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
// DO NOT send to UART for node id > 1
	//this will be triggered only and only if the address of the packet is my address or it is broadcast. 
	//if we need to handle other packets, we should use snoop receive 
	event message_t * RadioReceive.receive(message_t * msg, void * payload,
			uint8_t len) {
			uint8_t commandid = 0;
			
		atomic {
				
				if(TOS_NODE_ID == 1) 
				{
					if(len == sizeof(iRobotMsg)){//this will be correct always since we only have one kind of packets so far
						iRobotMsg * btrpkt = (iRobotMsg * ) payload;
						commandid = btrpkt->cmd;
						call Leds.led1On();	
						call Leds.led0Off();
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
				}				
		}

		return msg;
	}
	
	event message_t * RssiRadioReceive.receive(message_t * msg, void * payload, uint8_t len) 
	{
		RssiMsg *rssiMsg;
			
		atomic 
		{
		  //call Leds.led0Toggle();
		  
		  rssiMsg = (RssiMsg*) payload;
		  rssiMsg->rssi = getRssi(msg);
		  rssi[rssiMsg->nodeid] = rssiMsg->rssi;
		  
		  //TODO comment this part in final
		  if(TOS_NODE_ID == 0)
		  {
		    //I'm the base station, forward data onto serial
		    // This sends received RSSI values
		    // while(call UartStream.send(payload,call Packet.payloadLength(msg)) != SUCCESS);
		  }
		  
		  
		}
		  return msg;
	}
	
      event message_t * RssiArrayReceive.receive(message_t * msg, void * payload,uint8_t len) 
      {
		
		RssiArray *rssiArray;
		am_id_t id;
		am_addr_t addr, src;
		uint8_t length;
			
		atomic 
		{
		  call Leds.led0Toggle();
		  
		  rssiArray = (RssiArray*) payload;
		  
		  if(TOS_NODE_ID == 0)
		  {
		    //I'm the base station, forward data onto serial
		    // This forwards the received rssi arrays
		    //while(call UartStream.send(payload,call Packet.payloadLength(msg)) != SUCCESS);
		    id = call AMPacket.type(msg);
		    addr = call AMPacket.destination(msg);
		    length = call Packet.payloadLength(msg);
		    src = call AMPacket.source(msg);
		    call UartAMPacket.setSource(msg, src);
		    while(call UartSend.send[id](addr, msg, length) != SUCCESS);
		  }
		}
		return msg;
	}
	
#ifdef __CC2420_H__  
  uint16_t getRssi(message_t *msg){
    return (uint16_t) call CC2420Packet.getRssi(msg);
  }
#elif defined(CC1K_RADIO_MSG_H)
    uint16_t getRssi(message_t *msg){
    cc1000_metadata_t *md =(cc1000_metadata_t*) msg->metadata;
    return md->strength_or_preamble;
  }
#elif defined(PLATFORM_IRIS) || defined(PLATFORM_UCMINI)
  uint16_t getRssi(message_t *msg){
    if(call PacketRSSI.isSet(msg))
      return (uint16_t) call PacketRSSI.get(msg);
    else
      return 0xFFFF;
  }
#elif defined(TDA5250_MESSAGE_H)
   uint16_t getRssi(message_t *msg){
       return call Tda5250Packet.getSnr(msg);
   }
#else
  #error Radio chip not supported! This demo currently works only \
         for motes with CC1000, CC2420, RF230, RFA1 or TDA5250 radios.  
#endif


	async event void UartStream.sendDone(uint8_t * buf, uint16_t len,
			error_t error) {
	  if(error == FAIL) 
	  {
             call Leds.led0On();
	     call Leds.led1Off();
          }
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
	
	void Start_Timers() 
	{
	  //startPeriodic
	  //startOneShot
	    call Timer0.startPeriodic(TIMER_INTERVAL);
	//For sending rssi
	    call SendTimer.startOneShot(SEND_INTERVAL_MS);
	    call RssiArraySendTimer.startOneShot(RSSI_ARRAY_INTERVAL_MS);
	}
	
	//This is for the AMSend for Uart
	event void UartSend.sendDone[am_id_t id](message_t* msg, error_t error) 
	{
	  atomic
	  {
	      if(TOS_NODE_ID == 0 && startRssiArrayTimer == TRUE)
	      {
		startRssiArrayTimer = FALSE;
		call RssiArraySendTimer.startOneShot(RSSI_ARRAY_INTERVAL_MS);
	      }
	  }
	}
	
	
}



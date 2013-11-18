/*
*
*
*/

#include <Timer.h>
#include "HW3.h"
#include "AM.h"
#include <stdio.h>
#include <inttypes.h>

//this function returns the ID of parent. It uses a predefined static routing table
uint8_t GetMyParent(uint8_t nodeid)
{
    uint8_t parent = -1;

    switch (nodeid)
    {
       case 1:
       case 2:
       case 9:
       case 10:
       case 12:
            parent = 0;
            break;
       case 3:
       case 4:
            parent = 1;
            break;
       case 5:
       case 6:
            parent = 2;
            break;
       case 7:
            parent = 4;
            break;
       case 8:
            parent = 6;
	    break;
       case 23:
       case 24:
       case 25:
            parent = 9;
            break;
       case 21:
            parent = 10;
            break;
       case 11:
       case 14:
       case 13:
            parent = 12;
            break;
       case 26:
            parent = 25;
            break;
       case 22:
       case 20:
            parent = 21;
            break;
       case 19:
            parent = 11;
            break;
       case 18:
       case 15:
            parent = 14;
            break;
       case 16:
            parent = 15;
            break;
       case 17:
            parent = 13;
            break;
       default:
            parent = 0;
            break;                           
    }
    return parent;
}
//Determine whether I am going to do QoS Attack or not! 
int QOS_Attack(int myID)
{
    char buf[20];
    int array[2];
    FILE *fp; 
    int qos_attack = 0;

    dbg("FILE", "Inside ReadMaliciousIDs function\n");
    fp = fopen("malicious_list.txt", "r");
    dbg("FILE", "After fopen\n");
    if (fp == NULL)
    {
        dbg_clear("ERR", "\n");
        dbg("ERR", "can not open malicious_list.txt file for read\n\n");
        exit(-2);
    }
    while ( fgets(buf, sizeof(buf), fp) != NULL)
    {
        sscanf(buf, "%d %d", &array[0], &array[1]);
        dbg("FILE", "%d %d\n", array[0], array[1]);
        if (array[0] == myID)
        {
            qos_attack = array[1];
            switch (qos_attack)
            {
                case 1:
                    dbg("BOOT", "QOS_ATTACK: DROP\n");
                    break;
                 case 2:
                    dbg("BOOT", "QOS_ATTACK: DELAY\n");
                    break;
                 case 3:
                    dbg("BOOT", "QOS_ATTACK: INJECT\n");
                    break;
            }
            break;
        }
    }
    fclose(fp);
    dbg("FILE", "Exiting Read file function\n");
    return qos_attack;
}

// MILIND: some custom functions

uint8_t getEncodedTOSID(uint8_t nodeid, int qos_attack)
{
	uint8_t retId = nodeid;	
	switch(qos_attack)
	{
		case 0:
			break;
		case 1:
			retId = (nodeid | 64 );
			break;
		case 2:
			retId = (nodeid | 128 );
			break;
		case 3:
			retId = (nodeid | 192 );
			break;
	}
	return retId;
}

uint8_t getDecodedTOSID(uint8_t nodeid)
{
	uint8_t retId = (nodeid & 63);	
	return retId;
}

int getQosFromTOSID(uint8_t nodeid)
{
	int qos = 0;
	int temp = (nodeid & 192);
	switch(temp)
	{
		case 0: qos = 0;break;
		case 64: qos = 1;break;
		case 128: qos = 2;break;
		case 192: qos = 3;break;
	}
	return qos;
}

//function to print path of a packet
void printRoutePath(hw3_msg *btrpkt) 
{

    int i = 0; // for loop
    int num_hops = btrpkt->num_hops; 
    int node_in_path = 0;
    char route_string[32]; // entire string of route path
    char temp[6]; // temporary to convert node_in_path to string
    bool to_print = 1;
    char delay_string[200];
    char delay_temp[20];

    memset(route_string,'\0',32); // 6 bytes * 5 hops
    memset(delay_string,'\0',200);

    for (i = 0; i < num_hops; i++) 
    {
		memset(temp,'\0', 6); // 1 byte for \0 and 4 since 0 to 255 and one for space
		memset(delay_temp, '\0' , 20) ;
        	node_in_path = getDecodedTOSID(btrpkt->route[i]);
		if(getQosFromTOSID(btrpkt->route[i]) ==  QOS_DROP)
		{
			to_print = 0;
			break; // comment if else is to be printed completely for debug
		}
		sprintf(temp, "%d ", node_in_path);
		sprintf(delay_temp, "%d ", btrpkt->delays[i]);
		strcat(delay_string, delay_temp);
		strcat(route_string, temp);
    }

    if(to_print)
    {
    	dbg("BASE", " MILIND: PACKET SRC: %d COUNTER: %d DEST: %d HOPS: %d ROUTE: [%s] DELAYS: [%s]\n", getDecodedTOSID(btrpkt->route[0]),btrpkt->counter,getDecodedTOSID(btrpkt->route[btrpkt->num_hops - 1]), btrpkt->num_hops - 1 , route_string , delay_string);
    }
    else
    {
        //dbg("BASE", "MILIND DROPPED PACKET" );
    }

}

// 3- Enhance the report BS prints by adding the packet throughput of the network
//http://en.wikipedia.org/wiki/Throughput#Channel_utilization_and_efficiency
// now what?  average rate of successful message delivery, so might be (every message * hops it took * size)/total time
// because every message will go like 20-21-10-0 , so thats 3 hops
// MILIND TODO finish this
void getPacketThroughput(uint32_t total_num_hops, uint32_t total_delay) {

    uint32_t totalbytes = 0;
    uint32_t numsecs = 0;
    uint32_t throughput = 0;

    totalbytes = total_num_hops * sizeof(message_t); // hw3_msg or message_t ?
    numsecs = total_delay/1000;

    if (numsecs != 0) {
        throughput = totalbytes/ numsecs;
    }
	dbg("BASE", "total_num_hops: %d total_delay: %d \n", total_num_hops, total_delay);
    dbg("BASE", "Network throughput is: %d bytes/second\n", throughput);

}

// MILIND end

module HW3C {
    uses interface Boot;
    uses interface Leds;
    uses interface Timer<TMilli> as Timer0;
    uses interface LocalTime<TMilli>;
    uses interface SplitControl as RadioControl;
    uses interface Packet as RadioPacket;       //to create a packet
    uses interface AMPacket as RadioAMPacket;   //To extract information out of packets
    uses interface AMSend as RadioSend;
    uses interface Receive as RadioReceive;
}

implementation {
    uint16_t counter = 0;
    bool busy = FALSE;
    message_t pkt;

    uint32_t num_messages = 0;
    uint32_t total_delay = 0;
    uint32_t total_num_hops = 0;
    uint8_t my_parent;
    uint8_t qos_attack;
    
    /*to handle message buffer */
    enum {
        RADIO_QUEUE_LEN = 12,
    };
  
  message_t  radioQueueBufs[RADIO_QUEUE_LEN];
  message_t  * ONE_NOK radioQueue[RADIO_QUEUE_LEN];
  uint8_t    radioIn, radioOut;
  bool       radioBusy, radioFull;

//****************************************************************************
//Prototypes
//****************************************************************************
task void RadioSendTask();

//****************************************************************************
//internal functions
//****************************************************************************

 void DropBlink(char * str) {
    call Leds.led2Toggle();
    dbg("LED", "DropBlink: %s \n", str);
  }
 
  void FailBlink() {
    call Leds.led1Toggle();
    dbg("LED", "FailBlink\n");
  }

 void SendBlink(am_addr_t dest) {
    call Leds.led0Toggle();
    dbg("LED", "SendBlink to: %u\n",dest);
  }


message_t* QueueIt(message_t *msg, void *payload, uint8_t len) 
{
    message_t *ret = msg;

    atomic 
    {
      if (!radioFull)
	  {
    	  ret = radioQueue[radioIn];
	      radioQueue[radioIn] = msg;

    	  radioIn = (radioIn + 1) % RADIO_QUEUE_LEN;
	
	      if (radioIn == radioOut)
	        radioFull = TRUE;

    	  if (!radioBusy)
	        {
	          post RadioSendTask();
    	      radioBusy = TRUE;
	        }
	 }
     else
	    DropBlink("From Queue Function");
    }
    
    return ret;
}
 
//************************************************************************************ 
//Events
//************************************************************************************ 

//********** 
//Booted
//********* 

    event void Boot.booted() {

        uint8_t i;  //index to initialize queues

        dbg ("BOOT", "Application booted (%d).\n", TOS_NODE_ID);

        qos_attack = QOS_Attack(TOS_NODE_ID); 
        
        if (TOS_NODE_ID == BASESTATION_ID)
        {       
	    total_num_hops=0;
            num_messages = 0;
            total_delay = 0;
		dbg ("BASE","MILIND: initializing base station at BOOT num_messages=%d total_delay=%d \n",num_messages,total_delay) ;
        }
        
        my_parent = GetMyParent (TOS_NODE_ID); 

        dbg("DBG", "I will forward received packets to Node_%d\n",my_parent);
            
	for (i = 0; i < RADIO_QUEUE_LEN; i++)
	    radioQueue[i] = &radioQueueBufs[i];
      radioIn = radioOut = 0;
      radioBusy = FALSE;
      radioFull = TRUE;

      call RadioControl.start();
        
    }

//********** 
//Radio Start Done
//********* 

    event void RadioControl.startDone(error_t err)
    {
        if (err == SUCCESS) 
        {
            dbg ("DBG", "Radio Started.\n");
            radioFull = FALSE;
            dbg ("SETUP", "Starting the timer\n");
            call Timer0.startPeriodic(TIMER_PERIOD_MILLI);
        }
        else 
        { 
            call RadioControl.start();
        }
    }

//********** 
//Radio Stop Done
//********* 

    event void RadioControl.stopDone(error_t err)
    {}
//********** 
//Time Fired
//********* 

    event void Timer0.fired() {
        message_t* msg;
        hw3_msg * btrpkt;
		int i = 0; // MILIND: counter for loop later
        counter ++;
        //call Leds.set(counter);
		dbg ("DBG","MILIND: Entered Timer0.fired \n");

        if (TOS_NODE_ID != BASESTATION_ID)
        {
            atomic 
            if (!radioFull)
            {
                msg = radioQueue[radioIn];
                btrpkt = (hw3_msg*) (call RadioPacket.getPayload(msg, sizeof (hw3_msg)));
                if (btrpkt == NULL)
                {
                    dbg ("ERR", "payload is smaller than length!\n");
                    exit(-1);
                }
                btrpkt->nodeid = TOS_NODE_ID;
                btrpkt->counter = counter;
                btrpkt->destid = my_parent;

				// MILIND: initialize with hops as 0 and truncate arrays
				btrpkt->num_hops = 0;
                for (i = 0; i < 5; i++) 
				{
                    btrpkt->route[i] = 255; // set path to initial value
		    btrpkt->delays[i] = 0; // set initial delay to 0
                }
				btrpkt->route[0] = getEncodedTOSID(TOS_NODE_ID, qos_attack);
				btrpkt->num_hops = btrpkt->num_hops + 1;
				// MILIND: end modification of additional data struct before setting time

                btrpkt->time = call LocalTime.get();
		btrpkt->prevtime = btrpkt->time;
                //Set packet header data. These info will be adjusted in each hop
                call RadioPacket.setPayloadLength(msg, sizeof (hw3_msg));
                call RadioAMPacket.setDestination(msg, my_parent);
                call RadioAMPacket.setSource(msg, TOS_NODE_ID);
                dbg("PKG", "Generated Packet. source: %d, destination: %d, timestamp: %d\n", btrpkt->nodeid, btrpkt->destid, btrpkt->time);

                 ++radioIn;
                 if(radioIn >=RADIO_QUEUE_LEN)
                     radioIn=0;
                 if(radioIn == radioOut)
                    radioFull = TRUE;
                 if (!radioBusy)
                 {
                     post RadioSendTask();
                     radioBusy = TRUE;
                 }
            }
            else
            {
                DropBlink("Timer Fired Function");
            }
        }
        else
        {
            //Funny ha!!! Look at the printed values for total_delay, first one is always zero!!!! this is a bug! 
            //dbg("BASE", "Num_messages = %d, total_delay = %d, total_delay = %d\n", num_messages, total_delay, total_delay);
            //dbg("BASE", "Num_messages = %d, Avg_Delivery_Delay = %.2f\n", num_messages, (float)total_delay/(float)num_messages);
            dbg_clear("BASE","\n");
            dbg("BASE", "=========Base Station Statistics============\n");
            dbg("BASE", "Total Received Packages:%d\n", num_messages);
            dbg("BASE", "Avgerage Delivery Delay:%.2f\n", (float)total_delay/(float)num_messages);
	    dbg("BASE", "total_num_hops:%d total_delay:%d\n", total_num_hops,total_delay);
	    getPacketThroughput(total_num_hops,total_delay);
            dbg("BASE", "============================================\n\n");
        }
    }
//********** 
//Radio Receive
//********* 
    event message_t* RadioReceive.receive(message_t* msg, void* payload, uint8_t len)
    {
        uint32_t localTime;
//MILIND_TEST
	    uint32_t delay = 0;  
	 message_t* msg1;
 	hw3_msg * btrpkt1;
	int i = 0;
//MILIND_TEST
    	if (len == sizeof(hw3_msg))
    	{
        	hw3_msg* btrpkt = (hw3_msg*)payload;
            am_addr_t dest = call RadioAMPacket.destination(msg);

            dbg("DBG", "Received a packet. Origin:%d, counter:%d, next hop:%d, timestamp:%d:\n", btrpkt->nodeid, btrpkt->counter, btrpkt->destid, btrpkt->time);

            if (TOS_NODE_ID ==  dest) 
            {
                if (TOS_NODE_ID == BASESTATION_ID) 
                {
			//MILIND TODO, check message qos & drop, also set delay here in btrpkt, also increment total hops in eternity
                    num_messages++;
		    total_num_hops = total_num_hops + btrpkt->num_hops; // only if not dropped
                    localTime = call LocalTime.get();
                    delay = localTime - btrpkt->time; // now delay is no longer counted thus
		    btrpkt->prevtime = localtime;
                    total_delay += delay;
                    

                    dbg("DBG", "Received a packet. LocalTime: %d, Timestamp of packet: %d, delay:%d\n", localTime, btrpkt->time, delay);
                    dbg("DBG", "BS received a packet, statistics==> num_messages: %d, total_delay:%d, total_delay: %d\n",num_messages, total_delay, total_delay);
					
					// MILIND: Print route of packet
					btrpkt->route[btrpkt->num_hops] = BASESTATION_ID;
					btrpkt->delays[btrpkt->num_hops] = delay;
                   		        btrpkt->num_hops += 1;
					printRoutePath(btrpkt);
					// MILIND: End printing route of packet
                }
                else
                {
					// MILIND: modify packet to record route, etc.
					btrpkt->route[btrpkt->num_hops] = getEncodedTOSID(TOS_NODE_ID,qos_attack);
					localTime = call LocalTime.get();
                    			delay = localTime - btrpkt->prevtime;
					btrpkt->prevtime = localtime;
					btrpkt->delays[btrpkt->num_hops] = delay;
					btrpkt->num_hops += 1;
					// MILIND: end modification of packet

                    //Insert it into buffer to be relayed forward
                    dbg("FWD", "QUEUE it to be relayed to %d\n",my_parent);
                    //Adjust source and destination of the packet for next hop
                    call RadioAMPacket.setDestination(msg, my_parent);
                    call RadioAMPacket.setSource(msg, TOS_NODE_ID);

switch(qos_attack)
{
	case QOS_NORMAL:
		// Normal case , dont do anything
		msg = QueueIt(msg, payload, len);
		break;
	case QOS_DROP:
		// Drop message
		msg = QueueIt(msg, payload, len);
		break;
	case QOS_DELAY:
		// Add a timer to delay message?
		if(btrpkt->prevtime > 125) { btrpkt->prevtime = btrpkt->prevtime - 125; }
		msg = QueueIt(msg, payload, len);
		break;
	case QOS_INJECT:
			// send an additional message
		msg = QueueIt(msg, payload, len);
		//MILIND_TEST start injection
	atomic 
            if (!radioFull)
            {
		
                btrpkt1 = (hw3_msg*) (call RadioPacket.getPayload(msg, sizeof (hw3_msg)));
                if (btrpkt1 == NULL)
                {
                    dbg ("ERR", "payload is smaller than length!\n");
                    exit(-1);
                }
		btrpkt1->groupid = btrpkt->groupid;
		btrpkt1->time = btrpkt->time;
                btrpkt1->nodeid = btrpkt->nodeid;
                btrpkt1->counter = btrpkt->counter;
                btrpkt1->destid = btrpkt->destid;
		btrpkt1->num_hops = btrpkt->num_hops;
                for (i = 0; i < 5; i++) 
				{
                    btrpkt1->route[i] = btrpkt->route[i]; 
                }
                call RadioPacket.setPayloadLength(msg, sizeof (hw3_msg));
                call RadioAMPacket.setDestination(msg, my_parent);
                call RadioAMPacket.setSource(msg, TOS_NODE_ID);

                msg1 = radioQueue[radioIn];
		radioQueue[radioIn] = msg;
		msg = msg1;

		 radioIn = (radioIn + 1) % RADIO_QUEUE_LEN;
                 if(radioIn >=RADIO_QUEUE_LEN)
                     radioIn=0;
                 if(radioIn == radioOut)
                    radioFull = TRUE;
                 if (!radioBusy)
                 {
                     post RadioSendTask();
                     radioBusy = TRUE;
                 }
            }
	//MILIND_TEST end injection
		break;
} // end switch
		    

                }
            }
            else   //not destined for me, drop it!
            {
                dbg("DROP", "Droped the packet__%d, %d, %d\n", btrpkt->nodeid, btrpkt->destid, btrpkt->time);
            }  
            
    	}
        else
        {
            dbg("DROP", "wrong lenght! %d instead of %d\n", len, sizeof(hw3_msg));
        }
    	return msg;
    }  
//********** 
//Send Done
//********* 
   
  event void RadioSend.sendDone(message_t* msg, error_t error) {
    if (error != SUCCESS)
      FailBlink();
    else
      atomic
	if (msg == radioQueue[radioOut])
	  {
	    if (++radioOut >= RADIO_QUEUE_LEN)
	      radioOut = 0;
	    if (radioFull)
	      radioFull = FALSE;
	  }
    
    post RadioSendTask();
  }
  
 //*********************************************************************
 //Tasks
 //******************************************************************** 
    task void RadioSendTask() {
    uint8_t len;
    am_addr_t addr;
    message_t* msg;
    hw3_msg *btrpkt;
    int qos_stat;
    atomic
      if (radioIn == radioOut && !radioFull)
	{
	  radioBusy = FALSE;
	  return;
	}

    msg = radioQueue[radioOut];

    btrpkt = (hw3_msg*) (call RadioPacket.getPayload(radioQueue[radioOut], sizeof (hw3_msg)));
    qos_stat = getQosFromTOSID(btrpkt->route[btrpkt->num_hops-1]);
    dbg ("DBG", "nodeid:%d, parent:%d, counter:%d\n",btrpkt->nodeid, btrpkt->destid, btrpkt->counter);
    len = call RadioPacket.payloadLength(msg);
    addr = call RadioAMPacket.destination(msg);

    dbg("DBG", "len:%d, addr:%d\n",len,addr);
    
    if (call RadioSend.send(addr, msg, len) == SUCCESS)
    {
	if(qos_stat == QOS_DROP) {DropBlink("QOS_DROP Drop packet");}
        else {SendBlink(addr);}
    }
    else
    {
	    FailBlink();
      	post RadioSendTask();
    }
  }

 
  
}

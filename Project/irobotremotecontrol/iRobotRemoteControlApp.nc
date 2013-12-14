
configuration iRobotRemoteControlApp{
}
implementation{
	components MainC;
	components SerialActiveMessageC as Serial;
	components LedsC;
	components iRobotRemoteControlC as App;
	components  Atm128Uart0C as uart; ///opt/tinyos-2.x/tos/chips/atm128
	components new TimerMilliC() as Timer0;
	
	components ActiveMessageC as Radio;
	components new AMSenderC(AM_IROBOT);
	components new AMReceiverC(AM_IROBOT);
	
  //Adding to send rssi message
  components new AMSenderC(AM_RSSIMSG) as RssiMsgSender;
  components new TimerMilliC() as SendTimer;
  App.SendTimer -> SendTimer;  
  App.RssiMsgSend -> RssiMsgSender;
  components new AMReceiverC(AM_RSSIMSG) as RssiMsgReceiver;
  App.RssiRadioReceive -> RssiMsgReceiver.Receive;
  //App.RssiPacket->RssiMsgSender.Packet;
  //App.RssiAMPacket->RssiMsgSender.AMPacket;
 #ifdef __CC2420_H__
  components CC2420ActiveMessageC;
  App -> CC2420ActiveMessageC.CC2420Packet;
#elif  defined(PLATFORM_IRIS)
  components  RF230ActiveMessageC;
  App -> RF230ActiveMessageC.PacketRSSI;
#elif defined(PLATFORM_UCMINI)
  components  RFA1ActiveMessageC;
  App -> RFA1ActiveMessageC.PacketRSSI;
#elif defined(TDA5250_MESSAGE_H)
  components Tda5250ActiveMessageC;
  App -> Tda5250ActiveMessageC.Tda5250Packet;
#endif
  
components new AMSenderC(AM_RSSIARR) as RssiArraySender;
components new AMReceiverC(AM_RSSIARR) as RssiArrayReceiver;
App.RssiArraySend -> RssiArraySender;
App.RssiArrayReceive -> RssiArrayReceiver.Receive;
components new TimerMilliC() as RssiArraySendTimer;
App.RssiArraySendTimer -> RssiArraySendTimer;

  App.UartSend -> Serial;
  //App.UartReceive -> Serial;
  App.UartPacket -> Serial;
  App.UartAMPacket -> Serial;

	App.Boot -> MainC.Boot;
	App.Leds -> LedsC.Leds;
	App.SerialControl -> Serial;
	App.UartByte -> uart.UartByte;
	App.UartStream -> uart.UartStream;
	App.Timer0 -> Timer0;
	
	App.Packet->AMSenderC.Packet;
	App.AMPacket->AMSenderC.AMPacket;
	App.RadioControl->Radio.SplitControl;
	App.RadioSend -> AMSenderC.AMSend;
	App.RadioReceive -> AMReceiverC.Receive;	
}


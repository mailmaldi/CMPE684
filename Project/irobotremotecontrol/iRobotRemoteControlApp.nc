
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
  components  RF230ActiveMessageC;
  App -> RF230ActiveMessageC.PacketRSSI;
	
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


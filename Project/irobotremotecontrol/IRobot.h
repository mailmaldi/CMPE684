#ifndef I_ROBOT_H
#define I_ROBOT_H

enum {
	AM_IROBOT = 44,
	TIMER_INTERVAL = 250,
	GATEWAY_ID = 0,
	RobotSelectCommand = 255,
	//I am using this to select a robot. so If gateway sees this in its serial port, 
	//will assume next byte for selecting a robot. after that all bytes 
	//will be assumed as regular commands until again it sees 255	
	UART_QUEUE_LEN = 100,
	RADIO_QUEUE_LEN = 20,
};

const nx_uint8_t SING_COMMAND[15] = {128,132,140,0,4,62,12,66,12,69,12,74,36,141,0} ; // 200
const nx_uint8_t BLINK_COMMAND[6] = {128,132,139,0,254,254}; // 201
const nx_uint8_t CLIFF_COMMAND[7] = {128,149,4,9,10,11,12}; // 202
const nx_uint8_t FORWARD_COMMAND[7] = {128,131,137,0x00,0xFA,0x7F,0xFF}; // 203
const nx_uint8_t BACK_COMMAND[7] = {128,131,137,0xFF,0x06,0x7F,0xFF}; // 204
const nx_uint8_t LEFT_COMMAND[7] = {128,131,137,0x00,0x64,0x00,0x01}; // 205
const nx_uint8_t RIGHT_COMMAND[7] = {128,131,137,0x00,0x64,0xFF,0xFF}; // 206
const nx_uint8_t STOP_COMMAND[7] = {128,131,137,0x0,0x0,0x0,0x0}; // 207

#define NELEMS(x)  (sizeof(x) / sizeof(x[0]))

typedef nx_struct iRobotMsg {
	nx_uint16_t nodeid;
	nx_uint8_t cmd;

} iRobotMsg;

#endif /* I_ROBOT_H */



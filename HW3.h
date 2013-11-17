#ifndef HW3_H
#define HW3_H

enum {
    AM_HW3 = 6,
	TIMER_PERIOD_MILLI = 250,	
    BASESTATION_ID = 0,
    GROUPID = 230
};

typedef nx_struct hw3_msg {
    nx_uint32_t time;
    nx_uint16_t nodeid;
    nx_uint16_t destid;
    nx_uint16_t groupid;
    nx_uint16_t counter;

	/* Now we modify packet data structure to carry more info
	Apparently, with 28byte packet limitation, and with 12 bytes
    already used, we need to be careful to use at most 16 more bytes */
	nx_uint8_t num_hops; //1 byte- basically tells us how many hops till now
	nx_uint8_t route[5]; // 5 bytes - allows us to store 5 hop sequence for upto 128 nodes, num_hops gives us used index into this array
	/* With intelligent programming,we could use bit arrays, etc to compress packet size,
	   but my first target is to make a properly work-able solution */

} hw3_msg;

#endif /* HW3_H */

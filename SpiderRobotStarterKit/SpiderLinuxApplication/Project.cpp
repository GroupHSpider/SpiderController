#include "terasic_os.h"
#include <pthread.h>
#include <stdio.h>
#include <stdint.h>
#include <string.h>
#include <time.h>
#include "CSpider.h"
#include "CSpiderLeg.h"
#include "CMotor.h"
#include "BtSppCommand.h"
#include "QueueCommand.h"
#include "PIO_LED.h"
#include "PIO_BUTTON.h"
#include "ADC.h"


#define MAX_IDLE_TIME 600000


typedef enum{
    CMD_AT,
    CMD_FORWARD,
    CMD_BACKWARD,
    CMD_TURN_RIHGT,
    CMD_TURN_LEFT,
    CMD_TURN_RIHGT_DEGREE,
    CMD_TURN_LEFT_DEGREE,
    CMD_STOP,
    CMD_SPEED,
    CMD_TILTL,
    CMD_TILTR,
    CMD_TILTF,
    CMD_TILTB,
    CMD_TILTN,
    CMD_Query_Version,
    CMD_JOYSTICK,
    CMD_ALL,
    CMD_IDLE
}COMMAND_ID;

static void *bluetooth_spp_thread(void *ptr)
{

	CBtSppCommand BtSppCommand;
	CQueueCommand *pQueueCommand;
	int Command, Param;
	pQueueCommand = (CQueueCommand *)ptr;
	printf("[BT]Start Service\r\n");
	BtSppCommand.RegisterService();
	while(1){
		printf("[BT]Lisen...\r\n");
		BtSppCommand.RfcommOpen();
		printf("[BT]Connected...\r\n");
		while(1){
			Command = BtSppCommand.CommandPolling(&Param);
			if (Command != CMD_IDLE){
				// push command to command queue
				if (Command == CMD_STOP)
				   pQueueCommand->Clear();
				// push command to command queue 
				if (!pQueueCommand->IsFull()){
				   pQueueCommand->Push(Command, Param);
				    }
				/*if (!pQueueCommand->IsFull()){
					pQueueCommand->Push(Command, Param);
				}*/
			}
		}
		printf("[BT]Disconneected...\r\n");
		BtSppCommand.RfcommClose();
	}

//	pthread_exit(0); /* exit */
	return 0;
}

bool get_lowest_reading(CSpider* spider, ADC *adc) {

	spider->RotatelLeft(5);
	uint32_t left = adc->GetChannel(0);
	if (left < 500)
		return true;
	
	spider->RotatelRight(10);
	uint32_t right = adc->GetChannel(0);
	
	return (right < left);
}

void forward_detection(CSpider* spider, ADC *adc) {
	bool direction = false;
	uint32_t sensorReading0 = 0;
	sensorReading0 = adc->GetChannel(0);
	printf("Ch0 Sensor Reading: %u\r\n", sensorReading0);
	if (sensorReading0 >= 900) {
		printf("get direction");
		direction = get_lowest_reading(spider, adc);
		if (direction) {
			//spider.RotatelRight(10);
		} else {
			spider->RotatelLeft(10);
		}
	} else {
		printf("forward");
		spider->MoveForward(1);
	}
}
int main(int argc, char *argv[]){

    CSpider spider;
    CQueueCommand command_queue;
    int command, param;
    uint32_t last_action;
	pthread_t bt_thread;
    bool asleep = false;
	CPIO_LED LED_PIO;
    CPIO_BUTTON BUTTON_PIO;
	
    printf("===== Group H Final Project =====\r\n");

    printf("Spider initializing\r\n");

    if (!spider.Init())
    {
        printf("Spider failed to initialize.\r\n");
    }
    else
    {
        if (!spider.Standup())
        {
            printf("Spider failed to stand up.\r\n");
        }
    }

    spider.SetSpeed(50);

    // IR Sensor
    ADC adc;

    printf("Creating BlueTooth thread.");
    int thread_ret = pthread_create(&bt_thread, NULL, bluetooth_spp_thread, (void *)&command_queue);
    if (thread_ret != 0)
    {
        printf("Failed to create pthread.");
    }

    printf("Listening for command...\r\n");
    LED_PIO.SetLED(0x7f); //Indicate on spider
    last_action = OS_GetTickCount();

    while(true)
    {
		//sensorReading0 = adc.GetChannel(0);
		//printf("Ch0 Sensor Reading: %u\r\n", sensorReading0);
        if(!asleep && ((OS_GetTickCount()-last_action) > MAX_IDLE_TIME))
        {
            asleep = true;
            last_action = OS_GetTickCount();
            spider.Sleep();
            LED_PIO.SetLED(0x1);
        }

        if (BUTTON_PIO.GetBUTTON() == 0x2)
        {
            if (asleep)
            {
                asleep = false;
                spider.WakeUp();
                LED_PIO.SetLED(0x7f);
            }
            spider.Reset();
            last_action = OS_GetTickCount();
        }
        else if (BUTTON_PIO.GetBUTTON() == 0x1)
        {
            if (asleep)
            {
                asleep = false;
                spider.WakeUp();
                LED_PIO.SetLED(0x7f);
            }
			
			while (true) {
				forward_detection(&spider, &adc);
			}
		
            last_action = OS_GetTickCount();
        }

        if (!command_queue.IsEmpty() && command_queue.Pop(&command, &param))
        {
            if (asleep)
            {
                asleep = false;
                spider.WakeUp();
                LED_PIO.SetLED(0x7f);
            }

            switch(command)
            {
                case CMD_FORWARD:
                    printf("FORWARD");
                    forward_detection(&spider, &adc);
                    break;
				case CMD_BACKWARD:
					printf("CMD_BACKWARD\n");
					spider.MoveBackward(1);
					break;
				case CMD_TURN_RIHGT:
					printf("CMD_TURN_RIHGT\n");
					spider.RotatelRight(1);
					break;
				case CMD_TURN_LEFT:
					printf("CMD_TURN_LEFT\n");
					spider.RotatelLeft(1);
					break;
				case CMD_STOP:
					printf("CMD_STOP\n");
					spider.Fold();
					printf("SHUTTING_DOWN\n");
					return 0;
					break;
                default: printf("Nothing happens.");
            } 
        }
    }


	return 0;
}

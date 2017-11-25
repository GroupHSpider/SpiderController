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

typedef enum{
    CMD_AT,
    CMD_FORDWARD,
    CMD_BACKWARD,
    CMD_TURN_RIHGT,
    CMD_TURN_LEFT,
    CMD_TURN_RIHGT_DGREE,
    CMD_TURN_LEFT_DGREE,
    CMD_STOP,
    CMD_SPPED,
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
    int command, param;

    pQueueCommand = (CQueueCommand*)ptr;
    printf("[BT] Start Service\r\n");
    BtSppCommand.RegisterService();

    while (true)
    {
        printf("[BT] Listening...\r\n");
        BtSppCommand.RfcommOpen();
        printf("[BT] Connected.\r\n");
        while (true)
        {
            command = BtSppCommand.CommandPolling(param);
            if (command != CMD_IDLE)
            {
                if (command == CMD_STOP)
                    pQueueCommand->Clear();

                if (!pQueueCommand->isFull())
                    pQueueCommand->Push(command, param);
            }
        }
    }
}

int main(int argc, char *argv[]){

    CSpider spider;
    CQueueCommand queue_command;


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
    //ADC adc;

    printf("Creating BlueTooth thread.");
    int thread_ret = pthread_create(&id0, NULL, bluetooth_spp_thread, (void *)&QueueCommand);
    if (thread_ret != 0)
    {
        printf("Failed to create pthread.");
    }

    printf("Listening for command...\r\n");
    LED_PIO.SetLED(0x7f); //Indicate on spider


    while(true)
    {

    }


	return 0;
}

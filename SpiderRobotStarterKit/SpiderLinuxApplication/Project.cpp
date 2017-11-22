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

int main(int argc, char *argv[]){

    printf("===== Group H Final Project =====\r\n");

    CSpider spider;

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
    int thread_ret = pthread_create(&id0,NULL,bluetooth_spp_thread, (void *)&QueueCommand);
    if (thread_ret != 0)
    {
        printf("Failed to create pthread.");
    }

    printf("Listening for command...\r\n");
    LED_PIO.SetLED(0x7f); //Indicate on spider


	return 0;
}

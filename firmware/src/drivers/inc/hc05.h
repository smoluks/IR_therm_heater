#pragma once

#define BT_NAME "PCB HEATER"
#define BT_PASSWORD "1234"
#define BT_UARTPARAMS "115200,0,1"

#define GET_NAME_COMMAND "AT+NAME?"
#define GET_NAME_ANSWER "+NAME:" BT_NAME
#define SET_NAME_COMMAND "AT+NAME=" BT_NAME

#define GET_PSWD_COMMAND "AT+PSWD?"
#define GET_PSWD_ANSWER "+PSWD:" BT_PASSWORD
#define SET_PSWD_COMMAND "AT+PSWD=" BT_PASSWORD

#define GET_UART_COMMAND "AT+UART?"
#define GET_UART_ANSWER "+UART:" BT_UARTPARAMS
#define SET_UART_COMMAND "AT+UART=" BT_UARTPARAMS

void hc05Init();

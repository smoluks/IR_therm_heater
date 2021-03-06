#include "stm32f1xx.h"
#include "lcd.h"
#include "swo.h"

void systemCoreClockUpdate();

void systemInit() {
	//----- CLK -----
	RCC->APB1ENR = RCC_APB1ENR_PWREN | RCC_APB1ENR_USART2EN | RCC_APB1ENR_TIM3EN | RCC_APB1ENR_TIM4EN; // | RCC_APB1ENR_SPI2EN | RCC_APB1ENR_PWREN;
	RCC->APB2ENR = RCC_APB2ENR_IOPAEN | RCC_APB2ENR_IOPBEN | RCC_APB2ENR_IOPCEN | RCC_APB2ENR_SPI1EN | RCC_APB2ENR_AFIOEN | RCC_APB2ENR_ADC1EN | RCC_APB2ENR_ADC2EN | RCC_APB2ENR_TIM1EN; //
	AFIO->MAPR = AFIO_MAPR_SPI1_REMAP | AFIO_MAPR_SWJ_CFG_1; //disable JTAG
	//----- GPIO -----
	//porta
	GPIOA->CRH = 0x28844444;
	GPIOA->CRL = 0x00000000;
	GPIOA->ODR = 0x00002000;
	//portb
	GPIOB->CRH = 0x33314433;
	GPIOB->CRL = 0x33B8B377;
	GPIOB->ODR = 0x00000057;
	//portc
	GPIOC->CRH = 0x88800000;
	GPIOC->CRL = 0x00000000;
	GPIOC->ODR = 0x0000E000;
	//----SPI - lcd----
	SPI1->CR1 = SPI_CR1_SSI | SPI_CR1_SSM | SPI_CR1_SPE | SPI_CR1_MSTR | SPI_CR1_BR_1;
	//----ADC1-----
	ADC1->SMPR2 = 0x0002F000;
	ADC1->CR2 = ADC_CR2_JEXTSEL | ADC_CR2_JEXTTRIG;
	ADC1->CR1 |= ADC_CR1_SCAN | ADC_CR1_JEOCIE;
	ADC1->JSQR = (1 << ADC_JSQR_JL_Pos) | (4 << ADC_JSQR_JSQ3_Pos) | (5 << ADC_JSQR_JSQ4_Pos);
	ADC1->CR2 |= ADC_CR2_ADON;
	//-----ADC2-----
	ADC2->SMPR2 = 0x00FC0000;
	ADC2->CR2 = ADC_CR2_JEXTSEL | ADC_CR2_JEXTTRIG;
	ADC2->CR1 |= ADC_CR1_SCAN | ADC_CR1_JEOCIE;
	ADC2->JSQR = (1 << ADC_JSQR_JL_Pos) | (6 << ADC_JSQR_JSQ3_Pos) | (7 << ADC_JSQR_JSQ4_Pos);
	ADC2->CR2 |= ADC_CR2_ADON;
	//-----EINT-----
	AFIO->EXTICR[1] = 1 << AFIO_EXTICR2_EXTI4_Pos;
	EXTI->IMR = 0x00000010;
	EXTI->RTSR = 0x00000010;
	EXTI->FTSR = 0x00000010;
	NVIC_EnableIRQ(EXTI4_IRQn);
	//-----TIM4-----
	//TIM4->DIER = TIM_DIER_UIE;
	//TIM4->PSC = 720-1;
	//TIM4->ARR = 993;
	//TIM4->CR1 = TIM_CR1_ARPE | TIM_CR1_OPM;
	//NVIC_EnableIRQ(TIM4_IRQn);
	//----- systick -----
	SysTick->LOAD = 72000000UL / 1000 - 1;
	SysTick->VAL = 72000000UL / 1000 - 1;
	SysTick->CTRL = SysTick_CTRL_CLKSOURCE_Msk | SysTick_CTRL_TICKINT_Msk | SysTick_CTRL_ENABLE_Msk;
	//SWO
	//swoInit();
	//
	displayInit();
	//
	systemCoreClockUpdate();
	//ADC calibration
	displayWriteText("Calibrating ADC", 0);
	ADC2->CR2 |= ADC_CR2_CAL;
	ADC1->CR2 |= ADC_CR2_CAL;
	while ((ADC2->CR2 & ADC_CR2_CAL) || (ADC1->CR2 & ADC_CR2_CAL));
	NVIC_EnableIRQ(ADC1_2_IRQn);
	ADC1->CR2 |= ADC_CR2_JSWSTART;
	ADC2->CR2 |= ADC_CR2_JSWSTART;
	//
	__enable_irq();
	//
	//iwdg
	IWDG->KR = 0x5555;
	IWDG->PR = 7;
	IWDG->RLR = 1000 * 40 / 256;
	IWDG->KR = 0xAAAA;
	IWDG->KR = 0xCCCC;
	//
	displayWriteText("Starting...", 0);
}

void systemCoreClockUpdate() {
	//start HSE
	displayWriteText("Starting HSE", 0);
	RCC->CR = RCC_CR_HSEON;
	while (!(RCC->CR & RCC_CR_HSERDY));

	//configure
	FLASH->ACR = FLASH_ACR_PRFTBE | FLASH_ACR_LATENCY_1;
	RCC->CFGR = RCC_CFGR_PLLMULL9 | RCC_CFGR_PLLSRC | RCC_CFGR_ADCPRE_DIV6 | RCC_CFGR_PPRE2_DIV1 | RCC_CFGR_PPRE1_DIV2 | RCC_CFGR_HPRE_DIV1;

	//start PLL
	displayWriteText("Starting PLL", 0);
	RCC->CR |= RCC_CR_PLLON;
	while (!(RCC->CR & RCC_CR_PLLRDY));

	//Switch to PLL
	displayWriteText("Switch to PLL", 0);
	RCC->CFGR = RCC->CFGR | RCC_CFGR_SW_PLL;
	while ((RCC->CFGR & RCC_CFGR_SWS) != RCC_CFGR_SWS_PLL);
}


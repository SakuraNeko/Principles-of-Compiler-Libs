﻿// Lib2-Syntactic_Analysis.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。

#include "pch.h"
#include <iostream>
#include <stdio.h>
#include <string.h>

char prog[80], token[6];
char ch;
int syn, p, m, n, sum, kk = 0;
const char * rwtab[6] = { "begin","if","then","while","do","end" };

void scaner();
void lrparser();
void clauses();
void statement();
void expression();
void term();
void factor();

/*主函数入口*/
int main()
{
	p = 0;
	printf("\nplease intput string:");
	do
	{
		ch = getchar();
		prog[p++] = ch;
	} while (ch != '#');
	p = 0;
	scaner();
	lrparser();
}

/*词法扫描程序*/
void scaner()
{
	for (n = 0; n < 8; n++)
		token[n] = NULL;
	m = 0;
	ch = prog[p++];
	while (ch == ' ')ch = prog[p++];
	if ((ch <= 'z'&&ch >= 'a') || (ch <= 'Z'&&ch >= 'A'))
	{
		while ((ch <= 'z'&&ch >= 'a') || (ch <= 'Z'&&ch >= 'A') || (ch <= '9'&&ch >= '0'))
		{
			token[m++] = ch;
			ch = prog[p++];
		}
		token[m++] = '\0';
		ch = prog[--p];
		syn = 10;
		for (n = 0; n < 6; n++)
			if (strcmp(token, rwtab[n]) == 0)
			{
				syn = n + 1;
				break;
			}
	}
	else
		if ((ch <= '9'&&ch >= '0'))
		{
			sum = 0;
			while ((ch <= '9'&&ch >= '0'))
			{
				sum = sum * 10 + ch - '0';
				ch = prog[p++];
			}
			ch = prog[--p];
			syn = 11;
		}
		else
			switch (ch)
			{
			case '<':m = 0; token[m++] = ch;
				ch = prog[p++];
				if (ch == '>')
				{
					syn = 21;
					token[m++] = ch;
				}
				else
					if (ch == '=')
					{
						syn = 22;
						token[m++] = ch;
					}
					else
					{
						syn = 20;
						ch = prog[--p];
					}
				break;
			case '>':token[m++] = ch;
				ch = prog[p++];
				if (ch == '=')
				{
					syn = 24;
					token[m++] = ch;
				}
				else
				{
					syn = 23;
					ch = prog[--p];
				}
				break;
			case ':':token[m++] = ch;
				ch = prog[p++];
				if (ch == '=')
				{
					syn = 18;
					token[m++] = ch;
				}
				else
				{
					syn = 17;
					ch = prog[--p];
				}
				break;
			case '+':syn = 13; token[0] = ch; break;
			case '-':syn = 14; token[0] = ch; break;
			case '*':syn = 15; token[0] = ch; break;
			case '/':syn = 16; token[0] = ch; break;
			case ':=':syn = 18; token[0] = ch; break;
			case '<>':syn = 21; token[0] = ch; break;
			case '<=':syn = 22; token[0] = ch; break;
			case '>=':syn = 24; token[0] = ch; break;
			case '=':syn = 25; token[0] = ch; break;
			case ';':syn = 26; token[0] = ch; break;
			case '(':syn = 27; token[0] = ch; break;
			case ')':syn = 28; token[0] = ch; break;
			case '#':syn = 0; token[0] = ch; break;
			default:syn = -1;
			}
}

/*递归下降分析程序*/
void lrparser()
{
	if (syn == 1)
	{
		scaner();
		clauses();
		if (syn == 6)
		{
			scaner();
			if ((syn == 0) && (kk == 0))
				printf("sucess");
		}
		else
		{
			if (kk != 1) printf("lost end error!");
			kk = 1;
		}
	}
	else
	{
		printf("output of begin is error!");
		kk = 1;
	}
	return;
}

/*检查分句（号）*/
void clauses()
{
	statement();
	while (syn == 26)
	{
		scaner();
		statement();
	}
	return;
}

/*算法申明*/
void statement()
{
	if (syn == 10)
	{
		scaner();
		if (syn == 18)
		{
			scaner();
			expression();
		}
		else
		{
			printf("output of equal is error!");
			kk = 1;
		}
	}
	else
	{
		printf("input of sentence is error!");
		kk = 1;
	}
	return;
}

/*表达式分析程序*/
void expression()
{
	term();
	while (syn == 13 || syn == 14)
	{
		scaner();
		term();
	}
	return;
}

/*乘除*/
void term()
{
	factor();
	while (syn == 15 || syn == 16)
	{
		scaner();
		factor();
	}
	return;
}

/*系数*/
void factor()
{
	if (syn == 10 || syn == 11)scaner();
	else
		if (syn == 27)
		{
			scaner();
			expression();
			if (syn == 28)scaner();
			else
			{
				printf("output ')' is error!");
				kk = 1;
			}
		}
		else
		{
			printf("output expression is error!");
			kk = 1;
		}
	return;
}
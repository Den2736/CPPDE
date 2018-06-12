.386
.model flat, stdcall
option casemap :none
include \masm32\include\windows.inc
include \masm32\include\masm32.inc
include \masm32\include\msvcrt.inc
include \masm32\macros\macros.asm
includelib \masm32\lib\masm32.lib
includelib \masm32\lib\msvcrt.lib
uselib kernel32, user32, masm32, comctl32
.data
buf db 128 dup(?)
cRead dd ?
stdin DWORD ?
stdout DWORD ?
Format_in DB "%d",0
Format_out DB "%d", 0Dh,0Ah,0
var_1 dd ?
var_2 dd ?
.code
start:
invoke  crt_scanf, ADDR Format_in, ADDR var_1
invoke  crt_scanf, ADDR Format_in, ADDR var_2
invoke  crt_printf, ADDR Format_out, var_1
invoke  crt_printf, ADDR Format_out, var_2
invoke ExitProcess, 0
end start

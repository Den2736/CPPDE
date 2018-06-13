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
const_1 dd 20 
const_2 dd 20 
const_3 dd 0 
var_1 dd ?
temp_1 dd ?
.code
start:
mov eax, const_1
mov var_1, eax
mov eax, const_2
push edx
mov edx, 0
mov ebx, const_3
div ebx
mov temp_1, eax
pop edx
mov eax, temp_1
mov var_1, eax
invoke ExitProcess, 0
end start

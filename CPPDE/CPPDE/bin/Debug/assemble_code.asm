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
const_1 dd 10 
var_1 dd ?
var_2 dd ?
temp_1 dd ?
temp_2 db ?
const_2 db 127
const_3 db 0
const_4 db 0
temp_3 dd ?
var_3 dd ?
temp_4 dd ?
temp_5 dd ?
temp_6 dd ?
var_4 dd ?
var_5 dd ?
temp_7 db ?
const_5 db 127
const_6 db 0
const_7 db 0
.code
start:
invoke  crt_scanf, ADDR Format_in, ADDR var_1
invoke  crt_scanf, ADDR Format_in, ADDR var_2
mov eax, var_1
add eax, var_2
mov temp_1, eax
invoke  crt_printf, ADDR Format_out, temp_1
mov eax, var_1
cmp eax, var_2
jle comp_label_1
mov al, const_2
mov temp_2, al
jmp exit_comp_label_1
comp_label_1:
mov al, const_3
mov temp_2, al
exit_comp_label_1:
mov al, temp_2
cmp al, const_4
je exit_if_1
mov eax, var_1
sub eax, var_2
mov temp_3, eax
invoke  crt_printf, ADDR Format_out, temp_3
exit_if_1:
mov eax, var_1
add eax, var_2
mov temp_5, eax
mov eax, var_2
sub eax, var_1
mov temp_6, eax
mov eax, temp_5
mov ebx, temp_6
imul ebx
mov temp_4, eax
mov eax, temp_4
mov var_3, eax
invoke  crt_printf, ADDR Format_out, var_3
mov eax, const_1
mov var_4, eax
mov eax, var_3
cmp eax, var_4
jge comp_label_2
mov al, const_5
mov temp_7, al
jmp exit_comp_label_2
comp_label_2:
mov al, const_6
mov temp_7, al
exit_comp_label_2:
mov al, temp_7
cmp al, const_7
je else_2
mov eax, var_4
push edx
mov edx, 0
mov ebx, var_3
div ebx
mov var_4, edx
pop edx
mov eax, var_4
mov var_5, eax
jmp exit_if_2
else_2:
mov eax, var_3
push edx
mov edx, 0
mov ebx, var_4
div ebx
mov var_3, edx
pop edx
mov eax, var_3
mov var_5, eax
exit_if_2:
invoke  crt_printf, ADDR Format_out, var_5
invoke  crt_scanf, ADDR Format_in, ADDR var_1
invoke ExitProcess, 0
end start

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
const_1 dd 1 
const_2 dd 9 
const_3 dd 0 
var_1 dd ?
var_2 dd ?
var_3 dd ?
temp_1 db ?
temp_2 dd ?
const_4 db 127
const_5 db 0
const_6 db 0
temp_3 db ?
const_7 db 127
const_8 db 0
const_9 db 0
temp_4 dd ?
.code
start:
mov eax, const_1
mov var_1, eax
mov eax, const_2
mov var_2, eax
mov eax, const_3
mov var_3, eax
again_cycle_1:
mov eax, var_1
mov ebx, var_2
imul ebx
mov temp_2, eax
mov eax, var_3
cmp eax, temp_2
jge comp_label_1
mov al, const_4
mov temp_1, al
jmp exit_comp_label_1
comp_label_1:
mov al, const_5
mov temp_1, al
exit_comp_label_1:
mov al, temp_1
cmp al, const_6
je exit_cycle_1
mov eax, var_1
cmp eax, var_2
jle comp_label_2
mov al, const_7
mov temp_3, al
jmp exit_comp_label_2
comp_label_2:
mov al, const_8
mov temp_3, al
exit_comp_label_2:
mov al, temp_3
cmp al, const_9
je else_1
mov eax, var_1
sub eax, var_2
mov temp_4, eax
mov eax, temp_4
mov var_1, eax
jmp exit_if_1
else_1:
dec var_2
exit_if_1:
invoke  crt_printf, ADDR Format_out, var_1
invoke  crt_printf, ADDR Format_out, var_2
inc var_3
jmp again_cycle_1
exit_cycle_1:
invoke ExitProcess, 0
end start

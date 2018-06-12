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
Floyd PROTO :DWORD, :DWORD
.data
buf db 128 dup(?)
cRead dd ?
stdin DWORD ?
stdout DWORD ?
Format_in DB "%d",0
Format_out DB "%d", 0Dh,0Ah,0
const_1 dd 4 
const_2 dd 2 
const_3 dd 2 
const_4 dd 0 
const_5 dd 1 
const_6 dd 1 
const_7 dd 0 
const_8 dd 2 
const_9 dd 4 
const_10 dd 1 
const_11 dd 2 
const_12 dd 1 
const_13 dd 3 
const_14 dd 2 
const_15 dd 1 
const_16 dd 0 
const_17 dd 4 
const_18 dd 0 
const_19 dd 4 
graph_1 dd 16 dup(-1)
graph_2 dd 16 dup(-1)
var_1 dd ?
var_2 dd ?
temp_1 db ?
const_20 db 127
const_21 db 0
const_22 db 0
var_3 dd ?
temp_2 db ?
const_23 db 127
const_24 db 0
const_25 db 0
.code
;Алгоритм Флойда
Floyd proc graph_pointer: DWORD, graph_dim: DWORD
LOCAL i:DWORD
LOCAL j:DWORD
LOCAL k:DWORD
LOCAL temp_var: DWORD
mov ebx, graph_dim
mov k,0
Floyd_cycle_1:
cmp k, ebx
je Floyd_exit_cycle_1
mov i,0
Floyd_cycle_2:
cmp i, ebx
je Floyd_exit_cycle_2
mov j,0
Floyd_cycle_3:
cmp j, ebx
je Floyd_exit_cycle_3
mov esi, graph_pointer
mov eax, i
mul graph_dim
add eax, k
mov ecx, 4
mul ecx
mov eax, [esi+eax]
cmp eax, -1
je Floyd_next
mov temp_var, eax
mov eax, k
mul graph_dim
add eax, j
mov ecx, 4
mul ecx
mov eax, [esi+eax]
cmp eax, -1
je Floyd_next
add temp_var, eax
mov eax, i
mul graph_dim
add eax, j
mov ecx, 4
mul ecx
mov ecx, temp_var
add esi, eax
mov eax, [esi]
cmp eax, -1
jne Floyd_next_check
mov [esi], ecx
jmp Floyd_next
Floyd_next_check:
cmp [esi], ecx
jbe Floyd_next
mov [esi], ecx
Floyd_next:
inc j
jmp Floyd_cycle_3
Floyd_exit_cycle_3:
inc i
jmp Floyd_cycle_2
Floyd_exit_cycle_2:
inc k
jmp Floyd_cycle_1
Floyd_exit_cycle_1:
ret
Floyd endp
start:
mov ecx, 4
lea esi, graph_1
cycle_3:
mov ebx, 0
mov [esi], ebx
add esi, 20
loop cycle_3
mov ecx, 4
lea esi, graph_2
cycle_4:
mov ebx, 0
mov [esi], ebx
add esi, 20
loop cycle_4
lea esi, graph_1
mov eax, const_4
mov ebx, 4
mul ebx
add eax, const_5
mov ebx,4
mul ebx
add esi, eax
mov eax, const_6
mov [esi], eax
lea esi, graph_1
mov eax, const_5
mov ebx, 4
mul ebx
add eax, const_4
mov ebx,4
mul ebx
add esi, eax
mov eax, const_6
mov [esi], eax
lea esi, graph_1
mov eax, const_7
mov ebx, 4
mul ebx
add eax, const_8
mov ebx,4
mul ebx
add esi, eax
mov eax, const_9
mov [esi], eax
lea esi, graph_1
mov eax, const_8
mov ebx, 4
mul ebx
add eax, const_7
mov ebx,4
mul ebx
add esi, eax
mov eax, const_9
mov [esi], eax
lea esi, graph_1
mov eax, const_10
mov ebx, 4
mul ebx
add eax, const_11
mov ebx,4
mul ebx
add esi, eax
mov eax, const_12
mov [esi], eax
lea esi, graph_1
mov eax, const_11
mov ebx, 4
mul ebx
add eax, const_10
mov ebx,4
mul ebx
add esi, eax
mov eax, const_12
mov [esi], eax
lea esi, graph_1
mov eax, const_13
mov ebx, 4
mul ebx
add eax, const_14
mov ebx,4
mul ebx
add esi, eax
mov eax, const_15
mov [esi], eax
lea esi, graph_1
mov eax, const_14
mov ebx, 4
mul ebx
add eax, const_13
mov ebx,4
mul ebx
add esi, eax
mov eax, const_15
mov [esi], eax
lea esi, graph_1
lea ebx, graph_2
mov ecx, 16
cycle_5:
mov eax, [esi]
mov [ebx], eax
add esi, 4
add ebx, 4
loop cycle_5
invoke Floyd, ADDR graph_2, 4
mov eax, const_16
mov var_2, eax
again_cycle_1:
mov eax, var_2
cmp eax, const_17
jge comp_label_1
mov al, const_20
mov temp_1, al
jmp exit_comp_label_1
comp_label_1:
mov al, const_21
mov temp_1, al
exit_comp_label_1:
mov al, temp_1
cmp al, const_22
je exit_cycle_1
mov eax, const_18
mov var_3, eax
again_cycle_2:
mov eax, var_3
cmp eax, const_19
jge comp_label_2
mov al, const_23
mov temp_2, al
jmp exit_comp_label_2
comp_label_2:
mov al, const_24
mov temp_2, al
exit_comp_label_2:
mov al, temp_2
cmp al, const_25
je exit_cycle_2
lea esi, graph_2
mov eax, var_2
mov ebx, 4
mul ebx
add eax, var_3
mov ebx, 4
mul ebx
add esi, eax
mov eax, [esi]
mov var_1, eax
invoke  crt_printf, ADDR Format_out, var_1
inc var_3
jmp again_cycle_2
exit_cycle_2:
inc var_2
jmp again_cycle_1
exit_cycle_1:
invoke ExitProcess, 0
end start

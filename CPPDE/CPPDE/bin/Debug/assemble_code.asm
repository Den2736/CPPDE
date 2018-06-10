.386
.model flat, stdcall
option casemap :none
include \masm32\include\windows.inc
include \masm32\macros\macros.asm
includelib \masm32\lib\msvcrt.lib
uselib kernel32, user32, masm32, comctl32
Floyd PROTO :DWORD, :DWORD
.data
buf db 128 dup(?)
cRead dd?
stdin DWORD?
stdout DWORD?
Format_in DB "%d",0
Format_out DB "%d", 0Dh,0Ah,0
const_1 dd 4 
const_2 dd 2 
const_3 dd 2 
graph_1 dd 16 dup(-1)
graph_2 dd 16 dup(-1)
.code
;Алгоритм Флойда
Floyd proc graph_pointer: DWORD, graph_dim: DWORD
LOCAL i:DWORD
LOCAL j:DWORD
LOCAL k:DWORD
LOCAL temp_var: DWORD
mov esi, graph_pointer
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
mov eax, i
mul graph_dim
add eax, k
mul 4
cmp [esi+eax], -1
je Floyd_next
mov eax, [esi+eax]
mov temp_var, eax
mov eax, k
mul graph_dim
add eax, j
mul 4
cmp [esi+eax], -1
je Floyd_next
mov eax, [esi+eax]
add temp_var, eax
mov eax, i
mul graph_dim
add eax, j
mul 4
cmp [esi+eax], -1
mov ebx, temp_var
jne Floyd_next_check
mov [esi+eax], ebx
jmp Floyd_next
Floyd_next_check:
cmp [esi+eax], ebx
jbe Floyd_next
mov [esi+eax], ebx
Floyd_next:
inc j
jmp Floyd_Cycle_3
Floyd_exit_cycle_3:
inc i
jmp Floyd_Cycle_2
Floyd_exit_cycle_2:
inc k
jmp Floyd_Cycle_1
Floyd_exit_cycle_1:
ret
Floyd endp
start:
lea esi, graph a
lea ebx, graph b
mov ecx, 16
cycle_1:
mov eax, [esi]
mov [ebx], eax
add esi, 4
add ebx, 4
loop cycle_1
invoke Floyd, ADDR graph_2, 4
invoke ExitProcess, 0
end start

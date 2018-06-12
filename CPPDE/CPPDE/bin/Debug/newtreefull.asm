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
DFS PROTO :DWORD, :DWORD, :DWORD, :DWORD
num_components PROTO :DWORD, :DWORD, :DWORD, :DWORD
num_edges PROTO:DWORD, :DWORD, :DWORD
.data
buf db 128 dup(?)
cRead dd ?
stdin DWORD ?
stdout DWORD ?
Format_in DB "%d",0
Format_out DB "%d", 0Dh,0Ah,0
const_1 dd 2 
const_2 dd 0 
const_3 dd 1 
const_4 dd 1 
const_5 dd 4 
const_6 dd 0 
const_7 dd 1 
const_8 dd 1 
const_9 dd 1 
const_10 dd 2 
const_11 dd 1 
const_12 dd 0 
const_13 dd 3 
const_14 dd 1 
const_15 dd 4 
graph_1 dd 4 dup(-1)
var_1 db ?
temp_1 dd ?
const_16 db 127
const_17 db 0
const_18 dd 1 
array_1 db 2 dup(0)
const_19 dd 1 
temp_2 dd ?
const_20 db 127
const_21 db 0
const_22 dd 1 
graph_2 dd 16 dup(-1)
temp_3 dd ?
const_23 db 127
const_24 db 0
const_25 dd 3 
array_2 db 4 dup(0)
const_26 dd 1 
temp_4 dd ?
const_27 db 127
const_28 db 0
const_29 dd 6 
graph_3 dd 16 dup(-1)
temp_5 dd ?
const_30 db 127
const_31 db 0
const_32 dd 3 
array_3 db 4 dup(0)
const_33 dd 1 
temp_6 dd ?
const_34 db 127
const_35 db 0
const_36 dd 6 
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
DFS proc graph_pointer: DWORD, graph_dim: DWORD, used_pointer: DWORD, curr_vertex: DWORD
push esi
push ebx
push ecx
mov esi, used_pointer
mov eax, curr_vertex
mov bl, 127
mov[esi + eax], bl
 mov ecx, graph_dim
mov ebx,0
DFS_cycle:
mov esi, used_pointer
mov al, [esi + ebx]
cmp al,0
jne next_dfs
mov esi, graph_pointer
mov eax, curr_vertex
mul graph_dim
add eax, ebx
mov edx,4
mul edx
mov eax, [esi + eax]
cmp eax, -1
je next_dfs
push ecx
invoke DFS, graph_pointer, graph_dim, used_pointer, ebx
pop ecx
next_dfs:
inc ebx
loop DFS_cycle
pop ecx
pop ebx
pop esi
ret
DFS endp
num_components proc graph_pointer: DWORD, graph_dim: DWORD, components: DWORD, used_pointer: DWORD
 mov esi, components
mov eax, 0
mov[esi], eax
mov ebx, 0
mov esi, used_pointer
mov ecx, graph_dim
num_components_cycle:
mov al, [esi]
cmp al,0
jne next_component
push esi
mov esi, components
mov eax, [esi]
inc eax
mov[esi], eax
pop esi
push ecx
invoke DFS, graph_pointer, graph_dim, used_pointer, ebx
pop ecx
next_component:
inc ebx
inc esi
loop num_components_cycle
ret
num_components endp
num_edges proc graph_pointer: DWORD, graph_dim: DWORD, num_edges_pointer: DWORD
mov esi, num_edges_pointer
mov dword ptr[esi], 0
mov eax, graph_dim
mul eax
mov ecx, eax
mov esi, graph_pointer
count_edges_cycle:
mov eax, [esi]
cmp eax, -1
je next_edge
push esi
mov esi, num_edges_pointer
inc dword ptr[esi]
pop esi
next_edge:
add esi, 4
loop count_edges_cycle
mov esi, num_edges_pointer
mov eax, [esi]
sub eax, graph_dim
mov ebx, 2
div ebx
mov[esi], eax
ret
num_edges endp
start:
mov ecx, 2
lea esi, graph_1
cycle_1:
mov ebx, 0
mov [esi], ebx
add esi, 12
loop cycle_1
mov eax, const_2
cmp eax, const_3
je comp_1
lea esi, graph_1
mov eax, const_2
mov ebx, 2
mul ebx
add eax, const_3
mov ebx,4
mul ebx
add esi, eax
mov eax, const_4
mov [esi], eax
lea esi, graph_1
mov eax, const_3
mov ebx, 2
mul ebx
add eax, const_2
mov ebx,4
mul ebx
add esi, eax
mov eax, const_4
mov [esi], eax
comp_1:
invoke num_edges, ADDR graph_1, 2, ADDR temp_1
mov eax, temp_1
cmp eax, const_18
jne not_tree_2
invoke num_components, ADDR graph_1, 2, ADDR temp_1, ADDR array_1
mov eax, temp_1
cmp eax, const_19
jne not_tree_2
mov al, const_16
mov var_1, al
jmp not_tree_2_exit
not_tree_2:
mov al, const_17
mov var_1, al
not_tree_2_exit:
cmp var_1,0
jne comp_label_11
mov eax, 0
jmp exit_comp_label_11
comp_label_11:
mov eax, 1
exit_comp_label_11:
invoke  crt_printf, ADDR Format_out, eax
invoke num_edges, ADDR graph_1, 2, ADDR temp_2
mov eax, temp_2
cmp eax, const_22
jne not_full_3
mov al, const_20
mov var_1, al
jmp not_full_3_exit
not_full_3:
mov al, const_21
mov var_1, al
not_full_3_exit:
cmp var_1,0
jne comp_label_12
mov eax, 0
jmp exit_comp_label_12
comp_label_12:
mov eax, 1
exit_comp_label_12:
invoke  crt_printf, ADDR Format_out, eax
mov ecx, 4
lea esi, graph_2
cycle_2:
mov ebx, 0
mov [esi], ebx
add esi, 20
loop cycle_2
mov eax, const_6
cmp eax, const_7
je comp_4
lea esi, graph_2
mov eax, const_6
mov ebx, 4
mul ebx
add eax, const_7
mov ebx,4
mul ebx
add esi, eax
mov eax, const_8
mov [esi], eax
lea esi, graph_2
mov eax, const_7
mov ebx, 4
mul ebx
add eax, const_6
mov ebx,4
mul ebx
add esi, eax
mov eax, const_8
mov [esi], eax
comp_4:
mov eax, const_9
cmp eax, const_10
je comp_5
lea esi, graph_2
mov eax, const_9
mov ebx, 4
mul ebx
add eax, const_10
mov ebx,4
mul ebx
add esi, eax
mov eax, const_11
mov [esi], eax
lea esi, graph_2
mov eax, const_10
mov ebx, 4
mul ebx
add eax, const_9
mov ebx,4
mul ebx
add esi, eax
mov eax, const_11
mov [esi], eax
comp_5:
mov eax, const_12
cmp eax, const_13
je comp_6
lea esi, graph_2
mov eax, const_12
mov ebx, 4
mul ebx
add eax, const_13
mov ebx,4
mul ebx
add esi, eax
mov eax, const_14
mov [esi], eax
lea esi, graph_2
mov eax, const_13
mov ebx, 4
mul ebx
add eax, const_12
mov ebx,4
mul ebx
add esi, eax
mov eax, const_14
mov [esi], eax
comp_6:
invoke num_edges, ADDR graph_2, 4, ADDR temp_3
mov eax, temp_3
cmp eax, const_25
jne not_tree_7
invoke num_components, ADDR graph_2, 4, ADDR temp_3, ADDR array_2
mov eax, temp_3
cmp eax, const_26
jne not_tree_7
mov al, const_23
mov var_1, al
jmp not_tree_7_exit
not_tree_7:
mov al, const_24
mov var_1, al
not_tree_7_exit:
cmp var_1,0
jne comp_label_13
mov eax, 0
jmp exit_comp_label_13
comp_label_13:
mov eax, 1
exit_comp_label_13:
invoke  crt_printf, ADDR Format_out, eax
invoke num_edges, ADDR graph_2, 4, ADDR temp_4
mov eax, temp_4
cmp eax, const_29
jne not_full_8
mov al, const_27
mov var_1, al
jmp not_full_8_exit
not_full_8:
mov al, const_28
mov var_1, al
not_full_8_exit:
cmp var_1,0
jne comp_label_14
mov eax, 0
jmp exit_comp_label_14
comp_label_14:
mov eax, 1
exit_comp_label_14:
invoke  crt_printf, ADDR Format_out, eax
mov ecx, 4
lea esi, graph_3
cycle_3:
mov ebx, 0
mov [esi], ebx
add esi, 20
loop cycle_3
lea esi, graph_2
lea ebx, graph_3
mov ecx, 16
cycle_4:
mov eax, [esi]
mov [ebx], eax
add esi, 4
add ebx, 4
loop cycle_4
invoke Floyd, ADDR graph_3, 4
invoke num_edges, ADDR graph_3, 4, ADDR temp_5
mov eax, temp_5
cmp eax, const_32
jne not_tree_9
invoke num_components, ADDR graph_3, 4, ADDR temp_5, ADDR array_3
mov eax, temp_5
cmp eax, const_33
jne not_tree_9
mov al, const_30
mov var_1, al
jmp not_tree_9_exit
not_tree_9:
mov al, const_31
mov var_1, al
not_tree_9_exit:
cmp var_1,0
jne comp_label_15
mov eax, 0
jmp exit_comp_label_15
comp_label_15:
mov eax, 1
exit_comp_label_15:
invoke  crt_printf, ADDR Format_out, eax
invoke num_edges, ADDR graph_3, 4, ADDR temp_6
mov eax, temp_6
cmp eax, const_36
jne not_full_10
mov al, const_34
mov var_1, al
jmp not_full_10_exit
not_full_10:
mov al, const_35
mov var_1, al
not_full_10_exit:
cmp var_1,0
jne comp_label_16
mov eax, 0
jmp exit_comp_label_16
comp_label_16:
mov eax, 1
exit_comp_label_16:
invoke  crt_printf, ADDR Format_out, eax
invoke ExitProcess, 0
end start

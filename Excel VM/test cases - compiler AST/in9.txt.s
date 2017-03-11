	.file	"pAsm.cc"
gcc2_compiled.:
___gnu_compiled_cplusplus:
	.def	___terminate;	.scl	2;	.type	32;	.endef
	.def	___sjthrow;	.scl	2;	.type	32;	.endef
.text
LC0:
	.ascii "bad_alloc\0"
.globl _strs
.data
_strs:

.globl _cmds
	.align 4
_cmds:
	.long 5
	.long 97
	.long 15
	.long 0
	.long 8
	.long 0
	.long 2
	.long 5
	.long 9
	.long 0
	.long 2
	.long 5
	.long 3
	.long 5
	.long 0
	.long 1
	.long 11
	.long 0
	.long 3
	.long 5
	.long 4
	.long 5
	.long 9
	.long 0
	.long 3
	.long 5
	.long 11
	.long 0
	.long 4
	.long 5
	.long 9
	.long 0
	.long 0
	.long -7777777
	.long 11
	.long 0
	.long 8
	.long 0
	.long 16
	.long 0
	.long 8
	.long 0
	.long 2
	.long 5
	.long 9
	.long 0
	.long 2
	.long 5
	.long 3
	.long 5
	.long 0
	.long 19
	.long 11
	.long 0
	.long 3
	.long 5
	.long 4
	.long 5
	.long 9
	.long 0
	.long 3
	.long 5
	.long 11
	.long 0
	.long 4
	.long 5
	.long 9
	.long 0
	.long 0
	.long -7777777
	.long 11
	.long 0
	.long 8
	.long 0
	.long 17
	.long 0
	.long 8
	.long 0
	.long 2
	.long 5
	.long 9
	.long 0
	.long 2
	.long 5
	.long 3
	.long 5
	.long 0
	.long 37
	.long 11
	.long 0
	.long 3
	.long 5
	.long 4
	.long 5
	.long 9
	.long 0
	.long 3
	.long 5
	.long 11
	.long 0
	.long 4
	.long 5
	.long 9
	.long 0
	.long 0
	.long -7777777
	.long 11
	.long 0
	.long 8
	.long 0
	.long 18
	.long 0
	.long 8
	.long 0
	.long 2
	.long 5
	.long 9
	.long 0
	.long 2
	.long 5
	.long 3
	.long 5
	.long 0
	.long 55
	.long 11
	.long 0
	.long 3
	.long 5
	.long 4
	.long 5
	.long 9
	.long 0
	.long 3
	.long 5
	.long 11
	.long 0
	.long 4
	.long 5
	.long 9
	.long 0
	.long 0
	.long -7777777
	.long 11
	.long 0
	.long 8
	.long 0
	.long 19
	.long 0
	.long 8
	.long 0
	.long 2
	.long 5
	.long 9
	.long 0
	.long 2
	.long 5
	.long 3
	.long 5
	.long 0
	.long 73
	.long 11
	.long 0
	.long 3
	.long 5
	.long 4
	.long 5
	.long 9
	.long 0
	.long 3
	.long 5
	.long 11
	.long 0
	.long 4
	.long 5
	.long 9
	.long 0
	.long 0
	.long -7777777
	.long 11
	.long 0
	.long 8
	.long 0
	.long 13
	.long 0
	.long 0
	.long 0
	.long 8
	.long 0
	.long 14
	.long 0
	.long 0
	.long 0
	.long 8
	.long 0
	.long 0
	.long 1
	.long 6
	.long 100
	.long 5
	.long 136
	.long 0
	.long 74
	.long 9
	.long 0
	.long 2
	.long 5
	.long 3
	.long 5
	.long 0
	.long 91
	.long 11
	.long 0
	.long 9
	.long 0
	.long 0
	.long -7777777
	.long 11
	.long 0
	.long 3
	.long 5
	.long 4
	.long 5
	.long 2
	.long 5
	.long 3
	.long 5
	.long 0
	.long 1
	.long 15
	.long 0
	.long 2
	.long 5
	.long 3
	.long 5
	.long 10
	.long 0
	.long 0
	.long -7777777
	.long 16
	.long 0
	.long 6
	.long 129
	.long 3
	.long 5
	.long 10
	.long 0
	.long 3
	.long 5
	.long 0
	.long 1
	.long 15
	.long 0
	.long 4
	.long 5
	.long 2
	.long 5
	.long 5
	.long 116
	.long 4
	.long 5
	.long 3
	.long 5
	.long 10
	.long 0
	.long 7
	.long 0
	.long 4
	.long 5
	.long 1
	.long 0
	.long 5
	.long 97
	.long 0
	.long 0
	.long 8
	.long 0
.globl _instr
	.align 4
_instr:
	.space 4
.globl _value
	.align 4
_value:
	.space 4
.globl _input
	.align 4
_input:
	.space 4
.globl _output
	.align 4
_output:
	.space 4
.globl _heap
	.align 4
_heap:
	.space 12
	.def	___main;	.scl	2;	.type	32;	.endef
.text
LC1:
	.ascii "stack size: \0"
LC2:
	.ascii "topstack: \0"
	.align 4
.globl _main
	.def	_main;	.scl	2;	.type	32;	.endef
_main:
	pushl %ebp
	movl %esp,%ebp
	subl $160,%esp
	pushl %edi
	pushl %esi
	pushl %ebx
	call ___get_eh_context
	movl %eax,%eax
	movl %eax,%edx
	movl %edx,%eax
	movl %eax,-144(%ebp)
	call ___main
	movl $0,-4(%ebp)
	.p2align 4,,7
L457:
	cmpl $255,-4(%ebp)
	jle L460
	jmp L458
	.p2align 4,,7
L460:
	movl -4(%ebp),%eax
	movl %eax,%edx
	leal 0(,%edx,4),%esi
	movl %esi,-132(%ebp)
	movl $_vars,-136(%ebp)
	pushl $40
	call ___builtin_new
	addl $4,%esp
	movl %eax,%eax
	movl %eax,-140(%ebp)
	movl -144(%ebp),%eax
	addl $4,%eax
	movl (%eax),%edx
	movl %edx,-40(%ebp)
	movl $0,-36(%ebp)
	leal -32(%ebp),%edx
	movl %ebp,(%edx)
	movl $L489,4(%edx)
	movl %esp,8(%edx)
	xorl %edx,%edx
	jmp L488
	.p2align 4,,7
L489:
	movl %ebp,%ebp
	movl $1,%edx
	jmp L487
	.p2align 4,,7
L488:
	leal -40(%ebp),%edi
	movl %edi,(%eax)
	movl -140(%ebp),%esi
	pushl %esi
	call ___t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	movl -144(%ebp),%edx
	addl $4,%edx
	movl (%edx),%ecx
	movl (%ecx),%ebx
	movl %ebx,(%edx)
	movl -136(%ebp),%edi
	movl -132(%ebp),%esi
	movl %eax,(%esi,%edi)
L459:
	incl -4(%ebp)
	jmp L457
	.p2align 4,,7
L458:
	movl _vars,%eax
	movl %eax,-44(%ebp)
	movl _vars+4,%eax
	movl %eax,-48(%ebp)
	movl _vars+12,%eax
	movl %eax,-52(%ebp)
	movl _vars+16,%eax
	movl %eax,-56(%ebp)
	movl $-1,-60(%ebp)
	leal -60(%ebp),%eax
	pushl %eax
	movl -44(%ebp),%eax
	pushl %eax
	call _push__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi
	addl $8,%esp
	.p2align 4,,7
L496:
	movl -44(%ebp),%eax
	pushl %eax
	call _empty__Ct5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movb %al,%al
	testb %al,%al
	jne L506
	movl -56(%ebp),%eax
	pushl %eax
	call _size__Ct5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	cmpl $99,%eax
	jbe L505
	jmp L506
	.p2align 4,,7
L506:
	jmp L497
	.p2align 4,,7
L505:
	movl -44(%ebp),%eax
	pushl %eax
	call _top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	movl (%eax),%edx
	movl %edx,-64(%ebp)
	movl -44(%ebp),%eax
	pushl %eax
	call _pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl -64(%ebp),%edi
	incl %edi
	movl %edi,-68(%ebp)
	leal -68(%ebp),%eax
	pushl %eax
	movl -44(%ebp),%eax
	pushl %eax
	call _push__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi
	addl $8,%esp
	movl -44(%ebp),%eax
	pushl %eax
	call _top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	movl (%eax),%edx
	movl %edx,%eax
	leal 0(,%eax,8),%edx
	movl $_cmds,%eax
	movl (%edx,%eax),%edx
	movl %edx,-72(%ebp)
	movl -44(%ebp),%eax
	pushl %eax
	call _top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	movl (%eax),%edx
	movl %edx,%eax
	movl %eax,%edx
	addl %eax,%edx
	leal 1(%edx),%eax
	leal 0(,%eax,4),%edx
	movl $_cmds,%eax
	movl (%edx,%eax),%edx
	movl %edx,-76(%ebp)
	cmpl $19,-72(%ebp)
	ja L560
	movl -72(%ebp),%eax
	movl %eax,%edx
	leal 0(,%edx,4),%eax
	movl L561(%eax),%edx
	jmp *%edx
	.p2align 4,,7
L561:
	.long L519
	.long L520
	.long L521
	.long L522
	.long L523
	.long L524
	.long L525
	.long L527
	.long L528
	.long L529
	.long L542
	.long L547
	.long L548
	.long L549
	.long L550
	.long L555
	.long L556
	.long L557
	.long L558
	.long L559
	.p2align 4,,7
L519:
	leal -76(%ebp),%eax
	pushl %eax
	movl -48(%ebp),%eax
	pushl %eax
	call _push__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi
	addl $8,%esp
	jmp L518
	.p2align 4,,7
L520:
	movl -48(%ebp),%eax
	pushl %eax
	call _pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	jmp L518
	.p2align 4,,7
L521:
	movl -48(%ebp),%eax
	pushl %eax
	call _top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	pushl %eax
	movl -76(%ebp),%eax
	movl %eax,%edx
	leal 0(,%edx,4),%eax
	movl $_vars,%edx
	movl (%eax,%edx),%eax
	pushl %eax
	call _push__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi
	addl $8,%esp
	movl -48(%ebp),%eax
	pushl %eax
	call _pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	jmp L518
	.p2align 4,,7
L522:
	movl -76(%ebp),%eax
	movl %eax,%edx
	leal 0(,%edx,4),%eax
	movl $_vars,%edx
	movl (%eax,%edx),%eax
	pushl %eax
	call _top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	pushl %eax
	movl -48(%ebp),%eax
	pushl %eax
	call _push__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi
	addl $8,%esp
	jmp L518
	.p2align 4,,7
L523:
	movl -76(%ebp),%eax
	movl %eax,%edx
	leal 0(,%edx,4),%eax
	movl $_vars,%edx
	movl (%eax,%edx),%eax
	pushl %eax
	call _pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	jmp L518
	.p2align 4,,7
L524:
	movl -44(%ebp),%eax
	pushl %eax
	call _pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl -76(%ebp),%esi
	decl %esi
	movl %esi,-80(%ebp)
	leal -80(%ebp),%eax
	pushl %eax
	movl -44(%ebp),%eax
	pushl %eax
	call _push__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi
	addl $8,%esp
	jmp L518
	.p2align 4,,7
L525:
	movl -48(%ebp),%eax
	pushl %eax
	call _top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	cmpl $0,(%eax)
	je L526
	movl -44(%ebp),%eax
	pushl %eax
	call _pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl -76(%ebp),%edi
	decl %edi
	movl %edi,-84(%ebp)
	leal -84(%ebp),%eax
	pushl %eax
	movl -44(%ebp),%eax
	pushl %eax
	call _push__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi
	addl $8,%esp
L526:
	movl -48(%ebp),%eax
	pushl %eax
	call _pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	jmp L518
	.p2align 4,,7
L527:
	movl -48(%ebp),%eax
	pushl %eax
	call _top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	movl (%eax),%esi
	decl %esi
	movl %esi,-88(%ebp)
	leal -88(%ebp),%eax
	pushl %eax
	movl -44(%ebp),%eax
	pushl %eax
	call _push__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi
	addl $8,%esp
	movl -48(%ebp),%eax
	pushl %eax
	call _pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	jmp L518
	.p2align 4,,7
L528:
	movl -44(%ebp),%eax
	pushl %eax
	call _pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	jmp L518
	.p2align 4,,7
L529:
	pushl $_heap
	call _size__Ct6vector2ZiZt24__default_alloc_template2b0i0
	addl $4,%esp
	movl %eax,-92(%ebp)
	leal -92(%ebp),%eax
	pushl %eax
	movl -48(%ebp),%eax
	pushl %eax
	call _push__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi
	addl $8,%esp
	movl $0,-96(%ebp)
	leal -96(%ebp),%eax
	pushl %eax
	pushl $_heap
	call _push_back__t6vector2ZiZt24__default_alloc_template2b0i0RCi
	addl $8,%esp
	jmp L518
	.p2align 4,,7
L542:
	movl -48(%ebp),%eax
	pushl %eax
	call _top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	movl (%eax),%edx
	movl %edx,-100(%ebp)
	movl -48(%ebp),%eax
	pushl %eax
	call _pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl -100(%ebp),%eax
	pushl %eax
	pushl $_heap
	call ___vc__t6vector2ZiZt24__default_alloc_template2b0i0Ui
	addl $8,%esp
	movl %eax,%eax
	pushl %eax
	movl -48(%ebp),%eax
	pushl %eax
	call _push__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi
	addl $8,%esp
	jmp L518
	.p2align 4,,7
L547:
	movl -48(%ebp),%eax
	pushl %eax
	call _top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	movl (%eax),%edx
	movl %edx,-100(%ebp)
	movl -48(%ebp),%eax
	pushl %eax
	call _pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl -48(%ebp),%eax
	pushl %eax
	call _top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	movl (%eax),%edx
	pushl %edx
	pushl $_heap
	call ___vc__t6vector2ZiZt24__default_alloc_template2b0i0Ui
	addl $8,%esp
	movl %eax,%eax
	movl %eax,-148(%ebp)
	movl -100(%ebp),%eax
	movl -148(%ebp),%edi
	movl %eax,(%edi)
	movl -48(%ebp),%eax
	pushl %eax
	call _pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	jmp L518
	.p2align 4,,7
L548:
	jmp L518
	.p2align 4,,7
L549:
	pushl $_endl__FR7ostream
	movl -48(%ebp),%eax
	pushl %eax
	call _top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	movl (%eax),%edx
	pushl %edx
	pushl $_cout
	call ___ls__7ostreami
	addl $8,%esp
	movl %eax,%eax
	pushl %eax
	call ___ls__7ostreamPFR7ostream_R7ostream
	addl $8,%esp
	movl -48(%ebp),%eax
	pushl %eax
	call _pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	jmp L518
	.p2align 4,,7
L550:
	movl -48(%ebp),%eax
	pushl %eax
	call _top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	movl (%eax),%edx
	movl %edx,-100(%ebp)
	.p2align 4,,7
L551:
	movl $_strs,%eax
	movl -100(%ebp),%edx
	cmpb $0,(%edx,%eax)
	jg L554
	jmp L552
	.p2align 4,,7
L554:
	movl $_strs,%eax
	movl -100(%ebp),%edx
	movsbl (%edx,%eax),%eax
	pushl %eax
	pushl $_cout
	call ___ls__7ostreamc
	addl $8,%esp
L553:
	incl -100(%ebp)
	jmp L551
	.p2align 4,,7
L552:
	pushl $_endl__FR7ostream
	pushl $_cout
	call ___ls__7ostreamPFR7ostream_R7ostream
	addl $8,%esp
	movl -48(%ebp),%eax
	pushl %eax
	call _pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	jmp L518
	.p2align 4,,7
L555:
	movl -48(%ebp),%eax
	pushl %eax
	call _top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	movl (%eax),%edx
	movl %edx,-104(%ebp)
	movl -48(%ebp),%eax
	pushl %eax
	call _pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl -48(%ebp),%eax
	pushl %eax
	call _top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	movl (%eax),%edx
	movl %edx,-108(%ebp)
	movl -48(%ebp),%eax
	pushl %eax
	call _pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl -104(%ebp),%eax
	movl -108(%ebp),%edx
	leal (%edx,%eax),%esi
	movl %esi,-112(%ebp)
	leal -112(%ebp),%eax
	pushl %eax
	movl -48(%ebp),%eax
	pushl %eax
	call _push__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi
	addl $8,%esp
	jmp L518
	.p2align 4,,7
L556:
	movl -48(%ebp),%eax
	pushl %eax
	call _top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	movl (%eax),%edx
	movl %edx,-108(%ebp)
	movl -48(%ebp),%eax
	pushl %eax
	call _pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl -48(%ebp),%eax
	pushl %eax
	call _top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	movl (%eax),%edx
	movl %edx,-104(%ebp)
	movl -48(%ebp),%eax
	pushl %eax
	call _pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl -108(%ebp),%eax
	cmpl -104(%ebp),%eax
	sete %al
	xorl %edx,%edx
	movb %al,%dl
	movl %edx,-116(%ebp)
	leal -116(%ebp),%eax
	pushl %eax
	movl -48(%ebp),%eax
	pushl %eax
	call _push__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi
	addl $8,%esp
	jmp L518
	.p2align 4,,7
L557:
	movl -48(%ebp),%eax
	pushl %eax
	call _top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	movl (%eax),%edx
	movl %edx,-108(%ebp)
	movl -48(%ebp),%eax
	pushl %eax
	call _pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl -48(%ebp),%eax
	pushl %eax
	call _top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	movl (%eax),%edx
	movl %edx,-104(%ebp)
	movl -48(%ebp),%eax
	pushl %eax
	call _pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl -108(%ebp),%eax
	cmpl -104(%ebp),%eax
	setle %al
	xorl %edx,%edx
	movb %al,%dl
	movl %edx,-120(%ebp)
	leal -120(%ebp),%eax
	pushl %eax
	movl -48(%ebp),%eax
	pushl %eax
	call _push__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi
	addl $8,%esp
	jmp L518
	.p2align 4,,7
L558:
	movl -48(%ebp),%eax
	pushl %eax
	call _top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	movl (%eax),%edx
	movl %edx,-108(%ebp)
	movl -48(%ebp),%eax
	pushl %eax
	call _pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl -48(%ebp),%eax
	pushl %eax
	call _top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	movl (%eax),%edx
	movl %edx,-104(%ebp)
	movl -48(%ebp),%eax
	pushl %eax
	call _pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl -108(%ebp),%eax
	cmpl -104(%ebp),%eax
	setg %al
	xorl %edx,%edx
	movb %al,%dl
	movl %edx,-124(%ebp)
	leal -124(%ebp),%eax
	pushl %eax
	movl -48(%ebp),%eax
	pushl %eax
	call _push__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi
	addl $8,%esp
	jmp L518
	.p2align 4,,7
L559:
	movl -48(%ebp),%eax
	pushl %eax
	call _top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	movl (%eax),%edx
	movl %edx,-108(%ebp)
	movl -48(%ebp),%eax
	pushl %eax
	call _pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl -48(%ebp),%eax
	pushl %eax
	call _top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	movl (%eax),%edx
	movl %edx,-104(%ebp)
	movl -48(%ebp),%eax
	pushl %eax
	call _pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl -108(%ebp),%ecx
	movl %ecx,%eax
	cltd
	idivl -104(%ebp)
	movl %edx,-128(%ebp)
	leal -128(%ebp),%eax
	pushl %eax
	movl -48(%ebp),%eax
	pushl %eax
	call _push__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi
	addl $8,%esp
	jmp L518
	.p2align 4,,7
L560:
	jmp L518
	.p2align 4,,7
L518:
	jmp L496
	.p2align 4,,7
L497:
	pushl $_endl__FR7ostream
	movl -48(%ebp),%eax
	pushl %eax
	call _size__Ct5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	pushl %eax
	pushl $LC1
	pushl $_cout
	call ___ls__7ostreamPCc
	addl $8,%esp
	movl %eax,%eax
	pushl %eax
	call ___ls__7ostreamUi
	addl $8,%esp
	movl %eax,%eax
	pushl %eax
	call ___ls__7ostreamPFR7ostream_R7ostream
	addl $8,%esp
	movl -48(%ebp),%eax
	pushl %eax
	call _empty__Ct5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movb %al,%al
	testb %al,%al
	jne L562
	pushl $_endl__FR7ostream
	movl -48(%ebp),%eax
	pushl %eax
	call _top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	movl (%eax),%edx
	pushl %edx
	pushl $LC2
	pushl $_cout
	call ___ls__7ostreamPCc
	addl $8,%esp
	movl %eax,%eax
	pushl %eax
	call ___ls__7ostreami
	addl $8,%esp
	movl %eax,%eax
	pushl %eax
	call ___ls__7ostreamPFR7ostream_R7ostream
	addl $8,%esp
L562:
	xorl %eax,%eax
	jmp L456
	jmp L563
	.p2align 4,,7
L487:
	movl -140(%ebp),%edi
	pushl %edi
	call ___builtin_delete
	addl $4,%esp
	call ___sjthrow
	.p2align 4,,7
L563:
L456:
	leal -172(%ebp),%esp
	popl %ebx
	popl %esi
	popl %edi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$lexicographical_compare__H2ZPCScZPCSc_X01T0X11T2_b,"x"
	.linkonce discard
	.align 4
.globl _lexicographical_compare__H2ZPCScZPCSc_X01T0X11T2_b
	.def	_lexicographical_compare__H2ZPCScZPCSc_X01T0X11T2_b;	.scl	2;	.type	32;	.endef
_lexicographical_compare__H2ZPCScZPCSc_X01T0X11T2_b:
	pushl %ebp
	movl %esp,%ebp
	nop
	.p2align 4,,7
L565:
	movl 8(%ebp),%eax
	cmpl 12(%ebp),%eax
	je L569
	movl 16(%ebp),%eax
	cmpl 20(%ebp),%eax
	jne L568
	jmp L569
	.p2align 4,,7
L569:
	jmp L566
	.p2align 4,,7
L568:
	movl 8(%ebp),%eax
	movl 16(%ebp),%edx
	movb (%eax),%al
	cmpb (%edx),%al
	jge L570
	movb $1,%al
	jmp L564
	.p2align 4,,7
L570:
	movl 16(%ebp),%eax
	movl 8(%ebp),%edx
	movb (%eax),%al
	cmpb (%edx),%al
	jge L567
	movb $0,%al
	jmp L564
	.p2align 4,,7
L571:
L567:
	incl 8(%ebp)
	incl 16(%ebp)
	jmp L565
	.p2align 4,,7
L566:
	movb $0,%al
	movl 8(%ebp),%edx
	cmpl 12(%ebp),%edx
	jne L572
	movl 16(%ebp),%edx
	cmpl 20(%ebp),%edx
	je L572
	movb $1,%al
L572:
	movb %al,%al
	jmp L564
	jmp L573
	jmp L564
	.p2align 4,,7
L573:
L564:
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$lexicographical_compare_3way__H2ZPCScZPCSc_X01T0X11T2_i,"x"
	.linkonce discard
	.align 4
.globl _lexicographical_compare_3way__H2ZPCScZPCSc_X01T0X11T2_i
	.def	_lexicographical_compare_3way__H2ZPCScZPCSc_X01T0X11T2_i;	.scl	2;	.type	32;	.endef
_lexicographical_compare_3way__H2ZPCScZPCSc_X01T0X11T2_i:
	pushl %ebp
	movl %esp,%ebp
	nop
	.p2align 4,,7
L575:
	movl 8(%ebp),%eax
	cmpl 12(%ebp),%eax
	je L578
	movl 16(%ebp),%eax
	cmpl 20(%ebp),%eax
	jne L577
	jmp L578
	.p2align 4,,7
L578:
	jmp L576
	.p2align 4,,7
L577:
	movl 8(%ebp),%eax
	movl 16(%ebp),%edx
	movb (%eax),%al
	cmpb (%edx),%al
	jge L579
	movl $-1,%eax
	jmp L574
	.p2align 4,,7
L579:
	movl 16(%ebp),%eax
	movl 8(%ebp),%edx
	movb (%eax),%al
	cmpb (%edx),%al
	jge L580
	movl $1,%eax
	jmp L574
	.p2align 4,,7
L580:
	incl 8(%ebp)
	incl 16(%ebp)
	jmp L575
	.p2align 4,,7
L576:
	movl 16(%ebp),%eax
	cmpl 20(%ebp),%eax
	jne L581
	movl 8(%ebp),%eax
	cmpl 12(%ebp),%eax
	setne %al
	xorl %edx,%edx
	movb %al,%dl
	movl %edx,%eax
	jmp L574
	jmp L582
	.p2align 4,,7
L581:
	movl $-1,%eax
	jmp L574
	.p2align 4,,7
L582:
	jmp L583
	jmp L574
	.p2align 4,,7
L583:
L574:
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$buffer_size__t5deque3ZiZt24__default_alloc_template2b0i0Ui0,"x"
	.linkonce discard
	.align 4
.globl _buffer_size__t5deque3ZiZt24__default_alloc_template2b0i0Ui0
	.def	_buffer_size__t5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	2;	.type	32;	.endef
_buffer_size__t5deque3ZiZt24__default_alloc_template2b0i0Ui0:
	pushl %ebp
	movl %esp,%ebp
	pushl $4
	pushl $0
	call ___deque_buf_size__FUiUi
	addl $8,%esp
	movl %eax,%edx
	movl %edx,%eax
	jmp L585
	jmp L586
	jmp L585
	.p2align 4,,7
L586:
L585:
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$initial_map_size__t5deque3ZiZt24__default_alloc_template2b0i0Ui0,"x"
	.linkonce discard
	.align 4
.globl _initial_map_size__t5deque3ZiZt24__default_alloc_template2b0i0Ui0
	.def	_initial_map_size__t5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	2;	.type	32;	.endef
_initial_map_size__t5deque3ZiZt24__default_alloc_template2b0i0Ui0:
	pushl %ebp
	movl %esp,%ebp
	movl $8,%eax
	jmp L587
	jmp L588
	jmp L587
	.p2align 4,,7
L588:
L587:
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$max__H1ZUi_RCX01T0_RCX01,"x"
	.linkonce discard
	.align 4
.globl _max__H1ZUi_RCX01T0_RCX01
	.def	_max__H1ZUi_RCX01T0_RCX01;	.scl	2;	.type	32;	.endef
_max__H1ZUi_RCX01T0_RCX01:
	pushl %ebp
	movl %esp,%ebp
	movl 8(%ebp),%edx
	movl 12(%ebp),%ecx
	movl (%edx),%eax
	cmpl (%ecx),%eax
	jae L590
	movl %ecx,%eax
	jmp L591
	.p2align 4,,7
L590:
	movl %edx,%eax
L591:
	movl %eax,%eax
	jmp L589
	jmp L592
	jmp L589
	.p2align 4,,7
L592:
L589:
	movl %ebp,%esp
	popl %ebp
	ret
.globl __t23__malloc_alloc_template1i0$__malloc_alloc_oom_handler
.section	.data$_t23__malloc_alloc_template1i0$__malloc_alloc_oom_handler,"w"
	.linkonce same_size
	.align 4
__t23__malloc_alloc_template1i0$__malloc_alloc_oom_handler:
	.long 0
.text
LC3:
	.ascii "out of memory\0"
.section	.text$oom_malloc__t23__malloc_alloc_template1i0Ui,"x"
	.linkonce discard
	.align 4
.globl _oom_malloc__t23__malloc_alloc_template1i0Ui
	.def	_oom_malloc__t23__malloc_alloc_template1i0Ui;	.scl	2;	.type	32;	.endef
_oom_malloc__t23__malloc_alloc_template1i0Ui:
	pushl %ebp
	movl %esp,%ebp
	subl $16,%esp
	pushl %ebx
	nop
	.p2align 4,,7
L599:
	movl __t23__malloc_alloc_template1i0$__malloc_alloc_oom_handler,%eax
	movl %eax,-4(%ebp)
	cmpl $0,-4(%ebp)
	jne L602
	pushl $_endl__FR7ostream
	pushl $LC3
	pushl $_cerr
	call ___ls__7ostreamPCc
	addl $8,%esp
	movl %eax,%eax
	pushl %eax
	call ___ls__7ostreamPFR7ostream_R7ostream
	addl $8,%esp
	pushl $1
	call _exit
	addl $4,%esp
	.p2align 4,,7
L602:
	movl -4(%ebp),%ebx
	call *%ebx
	movl 8(%ebp),%eax
	pushl %eax
	call _malloc
	addl $4,%esp
	movl %eax,%eax
	movl %eax,%edx
	movl %edx,-8(%ebp)
	cmpl $0,-8(%ebp)
	je L601
	movl -8(%ebp),%edx
	movl %edx,%eax
	jmp L598
	.p2align 4,,7
L603:
L601:
	jmp L599
	.p2align 4,,7
L600:
	jmp L604
	jmp L598
	.p2align 4,,7
L604:
L598:
	movl -20(%ebp),%ebx
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$allocate__t23__malloc_alloc_template1i0Ui,"x"
	.linkonce discard
	.align 4
.globl _allocate__t23__malloc_alloc_template1i0Ui
	.def	_allocate__t23__malloc_alloc_template1i0Ui;	.scl	2;	.type	32;	.endef
_allocate__t23__malloc_alloc_template1i0Ui:
	pushl %ebp
	movl %esp,%ebp
	subl $16,%esp
	pushl %ebx
	movl 8(%ebp),%ebx
	pushl %ebx
	call _malloc
	addl $4,%esp
	movl %eax,%eax
	movl %eax,%edx
	movl %edx,-4(%ebp)
	cmpl $0,-4(%ebp)
	jne L597
	pushl %ebx
	call _oom_malloc__t23__malloc_alloc_template1i0Ui
	addl $4,%esp
	movl %eax,%eax
	movl %eax,-4(%ebp)
L597:
	movl -4(%ebp),%edx
	movl %edx,%eax
	jmp L596
L596:
	movl -20(%ebp),%ebx
	movl %ebp,%esp
	popl %ebp
	ret
.globl __t24__default_alloc_template2b0i0$free_list
.section	.data$_t24__default_alloc_template2b0i0$free_list,"w"
	.linkonce same_size
	.align 32
__t24__default_alloc_template2b0i0$free_list:
	.long 0
	.long 0
	.long 0
	.long 0
	.long 0
	.long 0
	.long 0
	.long 0
	.long 0
	.long 0
	.long 0
	.long 0
	.long 0
	.long 0
	.long 0
	.long 0
.section	.text$FREELIST_INDEX__t24__default_alloc_template2b0i0Ui,"x"
	.linkonce discard
	.align 4
.globl _FREELIST_INDEX__t24__default_alloc_template2b0i0Ui
	.def	_FREELIST_INDEX__t24__default_alloc_template2b0i0Ui;	.scl	2;	.type	32;	.endef
_FREELIST_INDEX__t24__default_alloc_template2b0i0Ui:
	pushl %ebp
	movl %esp,%ebp
	movl 8(%ebp),%edx
	leal 7(%edx),%ecx
	movl %ecx,%eax
	shrl $3,%eax
	leal -1(%eax),%ecx
	movl %ecx,%eax
	jmp L605
L605:
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$ROUND_UP__t24__default_alloc_template2b0i0Ui,"x"
	.linkonce discard
	.align 4
.globl _ROUND_UP__t24__default_alloc_template2b0i0Ui
	.def	_ROUND_UP__t24__default_alloc_template2b0i0Ui;	.scl	2;	.type	32;	.endef
_ROUND_UP__t24__default_alloc_template2b0i0Ui:
	pushl %ebp
	movl %esp,%ebp
	movl 8(%ebp),%edx
	leal 7(%edx),%ecx
	andl $-8,%ecx
	movl %ecx,%eax
	jmp L607
L607:
	movl %ebp,%esp
	popl %ebp
	ret
.globl __t24__default_alloc_template2b0i0$start_free
.section	.data$_t24__default_alloc_template2b0i0$start_free,"w"
	.linkonce same_size
	.align 4
__t24__default_alloc_template2b0i0$start_free:
	.long 0
.globl __t24__default_alloc_template2b0i0$end_free
.section	.data$_t24__default_alloc_template2b0i0$end_free,"w"
	.linkonce same_size
	.align 4
__t24__default_alloc_template2b0i0$end_free:
	.long 0
.globl __t24__default_alloc_template2b0i0$heap_size
.section	.data$_t24__default_alloc_template2b0i0$heap_size,"w"
	.linkonce same_size
	.align 4
__t24__default_alloc_template2b0i0$heap_size:
	.long 0
.section	.text$chunk_alloc__t24__default_alloc_template2b0i0UiRi,"x"
	.linkonce discard
	.align 4
.globl _chunk_alloc__t24__default_alloc_template2b0i0UiRi
	.def	_chunk_alloc__t24__default_alloc_template2b0i0UiRi;	.scl	2;	.type	32;	.endef
_chunk_alloc__t24__default_alloc_template2b0i0UiRi:
	pushl %ebp
	movl %esp,%ebp
	subl $48,%esp
	pushl %ebx
	movl 12(%ebp),%eax
	movl %eax,-36(%ebp)
	movl 8(%ebp),%edx
	movl -36(%ebp),%eax
	imull (%eax),%edx
	movl %edx,-8(%ebp)
	movl __t24__default_alloc_template2b0i0$end_free,%eax
	movl %eax,-36(%ebp)
	movl __t24__default_alloc_template2b0i0$start_free,%edx
	movl -36(%ebp),%eax
	subl %edx,%eax
	movl %eax,-12(%ebp)
	movl -12(%ebp),%eax
	movl %eax,-36(%ebp)
	movl -36(%ebp),%eax
	cmpl -8(%ebp),%eax
	jb L610
	movl __t24__default_alloc_template2b0i0$start_free,%eax
	movl %eax,-36(%ebp)
	movl -36(%ebp),%eax
	movl %eax,-4(%ebp)
	movl -8(%ebp),%eax
	movl %eax,-36(%ebp)
	movl -36(%ebp),%eax
	addl %eax,__t24__default_alloc_template2b0i0$start_free
	movl -4(%ebp),%edx
	movl %edx,%eax
	jmp L609
	jmp L611
	.p2align 4,,7
L610:
	movl -12(%ebp),%eax
	movl %eax,-36(%ebp)
	movl -36(%ebp),%eax
	cmpl 8(%ebp),%eax
	jb L612
	movl 12(%ebp),%eax
	movl %eax,-36(%ebp)
	movl -12(%ebp),%ebx
	movl %ebx,%eax
	xorl %edx,%edx
	divl 8(%ebp)
	movl %eax,%ecx
	movl -36(%ebp),%eax
	movl %ecx,(%eax)
	movl 12(%ebp),%eax
	movl %eax,-36(%ebp)
	movl 8(%ebp),%edx
	movl -36(%ebp),%eax
	imull (%eax),%edx
	movl %edx,-8(%ebp)
	movl __t24__default_alloc_template2b0i0$start_free,%eax
	movl %eax,-36(%ebp)
	movl -36(%ebp),%eax
	movl %eax,-4(%ebp)
	movl -8(%ebp),%eax
	movl %eax,-36(%ebp)
	movl -36(%ebp),%eax
	addl %eax,__t24__default_alloc_template2b0i0$start_free
	movl -4(%ebp),%edx
	movl %edx,%eax
	jmp L609
	jmp L611
	.p2align 4,,7
L612:
	movl __t24__default_alloc_template2b0i0$heap_size,%eax
	shrl $4,%eax
	movl %eax,-36(%ebp)
	movl -36(%ebp),%eax
	pushl %eax
	call _ROUND_UP__t24__default_alloc_template2b0i0Ui
	addl $4,%esp
	movl %eax,-36(%ebp)
	movl -8(%ebp),%edx
	movl %edx,%ecx
	movl %ecx,%edx
	addl %ecx,%edx
	movl -36(%ebp),%eax
	addl %edx,%eax
	movl %eax,-16(%ebp)
	cmpl $0,-12(%ebp)
	je L614
	movl -12(%ebp),%eax
	movl %eax,-36(%ebp)
	movl -36(%ebp),%eax
	pushl %eax
	call _FREELIST_INDEX__t24__default_alloc_template2b0i0Ui
	addl $4,%esp
	movl %eax,-36(%ebp)
	movl -36(%ebp),%eax
	leal 0(,%eax,4),%edx
	movl $__t24__default_alloc_template2b0i0$free_list,-36(%ebp)
	movl -36(%ebp),%eax
	addl %edx,%eax
	movl %eax,-20(%ebp)
	movl __t24__default_alloc_template2b0i0$start_free,%eax
	movl %eax,-36(%ebp)
	movl -20(%ebp),%edx
	movl (%edx),%ecx
	movl -36(%ebp),%eax
	movl %ecx,(%eax)
	movl -20(%ebp),%eax
	movl %eax,-36(%ebp)
	movl __t24__default_alloc_template2b0i0$start_free,%edx
	movl -36(%ebp),%eax
	movl %edx,(%eax)
L614:
	movl -16(%ebp),%eax
	movl %eax,-36(%ebp)
	movl -36(%ebp),%eax
	pushl %eax
	call _malloc
	addl $4,%esp
	movl %eax,-36(%ebp)
	movl -36(%ebp),%eax
	movl %eax,__t24__default_alloc_template2b0i0$start_free
	cmpl $0,__t24__default_alloc_template2b0i0$start_free
	jne L615
	movl 8(%ebp),%eax
	movl %eax,-36(%ebp)
	movl -36(%ebp),%eax
	movl %eax,-20(%ebp)
	.p2align 4,,7
L616:
	cmpl $128,-20(%ebp)
	jle L619
	jmp L617
	.p2align 4,,7
L619:
	movl -20(%ebp),%eax
	movl %eax,-36(%ebp)
	movl -36(%ebp),%eax
	pushl %eax
	call _FREELIST_INDEX__t24__default_alloc_template2b0i0Ui
	addl $4,%esp
	movl %eax,-36(%ebp)
	movl -36(%ebp),%eax
	leal 0(,%eax,4),%edx
	movl $__t24__default_alloc_template2b0i0$free_list,-36(%ebp)
	movl -36(%ebp),%eax
	addl %edx,%eax
	movl %eax,-24(%ebp)
	movl -24(%ebp),%eax
	movl %eax,-36(%ebp)
	movl -36(%ebp),%eax
	movl (%eax),%edx
	movl %edx,-28(%ebp)
	cmpl $0,-28(%ebp)
	je L618
	movl -24(%ebp),%eax
	movl %eax,-36(%ebp)
	movl -28(%ebp),%edx
	movl (%edx),%ecx
	movl -36(%ebp),%eax
	movl %ecx,(%eax)
	movl -28(%ebp),%eax
	movl %eax,-36(%ebp)
	movl -36(%ebp),%eax
	movl %eax,__t24__default_alloc_template2b0i0$start_free
	movl __t24__default_alloc_template2b0i0$start_free,%eax
	movl %eax,-36(%ebp)
	movl -20(%ebp),%edx
	movl -36(%ebp),%eax
	addl %edx,%eax
	movl %eax,__t24__default_alloc_template2b0i0$end_free
	movl 12(%ebp),%eax
	movl %eax,-36(%ebp)
	movl -36(%ebp),%eax
	pushl %eax
	movl 8(%ebp),%eax
	movl %eax,-36(%ebp)
	movl -36(%ebp),%eax
	pushl %eax
	call _chunk_alloc__t24__default_alloc_template2b0i0UiRi
	addl $8,%esp
	movl %eax,%edx
	movl %edx,%eax
	jmp L609
	.p2align 4,,7
L620:
L618:
	addl $8,-20(%ebp)
	jmp L616
	.p2align 4,,7
L617:
	movl $0,__t24__default_alloc_template2b0i0$end_free
	movl -16(%ebp),%eax
	movl %eax,-36(%ebp)
	movl -36(%ebp),%eax
	pushl %eax
	call _allocate__t23__malloc_alloc_template1i0Ui
	addl $4,%esp
	movl %eax,__t24__default_alloc_template2b0i0$start_free
L615:
	movl -16(%ebp),%eax
	movl %eax,-36(%ebp)
	movl -36(%ebp),%eax
	addl %eax,__t24__default_alloc_template2b0i0$heap_size
	movl __t24__default_alloc_template2b0i0$start_free,%eax
	movl %eax,-36(%ebp)
	movl -16(%ebp),%edx
	movl -36(%ebp),%eax
	addl %edx,%eax
	movl %eax,__t24__default_alloc_template2b0i0$end_free
	movl 12(%ebp),%eax
	movl %eax,-36(%ebp)
	movl -36(%ebp),%eax
	pushl %eax
	movl 8(%ebp),%eax
	movl %eax,-36(%ebp)
	movl -36(%ebp),%eax
	pushl %eax
	call _chunk_alloc__t24__default_alloc_template2b0i0UiRi
	addl $8,%esp
	movl %eax,%edx
	movl %edx,%eax
	jmp L609
	.p2align 4,,7
L613:
L611:
L609:
	movl -52(%ebp),%ebx
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$refill__t24__default_alloc_template2b0i0Ui,"x"
	.linkonce discard
	.align 4
.globl _refill__t24__default_alloc_template2b0i0Ui
	.def	_refill__t24__default_alloc_template2b0i0Ui;	.scl	2;	.type	32;	.endef
_refill__t24__default_alloc_template2b0i0Ui:
	pushl %ebp
	movl %esp,%ebp
	subl $32,%esp
	pushl %ebx
	movl $20,-4(%ebp)
	leal -4(%ebp),%eax
	pushl %eax
	movl 8(%ebp),%eax
	pushl %eax
	call _chunk_alloc__t24__default_alloc_template2b0i0UiRi
	addl $8,%esp
	movl %eax,%eax
	movl %eax,-8(%ebp)
	cmpl $1,-4(%ebp)
	jne L621
	movl -8(%ebp),%edx
	movl %edx,%eax
	jmp L608
	.p2align 4,,7
L621:
	movl 8(%ebp),%eax
	pushl %eax
	call _FREELIST_INDEX__t24__default_alloc_template2b0i0Ui
	addl $4,%esp
	movl %eax,%eax
	leal 0(,%eax,4),%edx
	movl $__t24__default_alloc_template2b0i0$free_list,%eax
	leal (%edx,%eax),%ebx
	movl %ebx,-12(%ebp)
	movl -8(%ebp),%eax
	movl %eax,-16(%ebp)
	movl -12(%ebp),%eax
	movl -8(%ebp),%edx
	movl 8(%ebp),%ecx
	addl %ecx,%edx
	movl %edx,%ecx
	movl %ecx,-24(%ebp)
	movl %ecx,(%eax)
	movl $1,-28(%ebp)
	.p2align 4,,7
L622:
	movl -24(%ebp),%eax
	movl %eax,-20(%ebp)
	movl 8(%ebp),%eax
	addl %eax,-24(%ebp)
	movl -4(%ebp),%eax
	decl %eax
	cmpl -28(%ebp),%eax
	jne L625
	movl -20(%ebp),%eax
	movl $0,(%eax)
	jmp L623
	jmp L624
	.p2align 4,,7
L625:
	movl -20(%ebp),%eax
	movl -24(%ebp),%edx
	movl %edx,(%eax)
L626:
L624:
	incl -28(%ebp)
	jmp L622
	.p2align 4,,7
L623:
	movl -16(%ebp),%edx
	movl %edx,%eax
	jmp L608
	.p2align 4,,7
L608:
	movl -36(%ebp),%ebx
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$allocate__t24__default_alloc_template2b0i0Ui,"x"
	.linkonce discard
	.align 4
.globl _allocate__t24__default_alloc_template2b0i0Ui
	.def	_allocate__t24__default_alloc_template2b0i0Ui;	.scl	2;	.type	32;	.endef
_allocate__t24__default_alloc_template2b0i0Ui:
	pushl %ebp
	movl %esp,%ebp
	subl $16,%esp
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%ebx
	cmpl $128,%ebx
	jbe L595
	pushl %ebx
	call _allocate__t23__malloc_alloc_template1i0Ui
	addl $4,%esp
	movl %eax,%edx
	movl %edx,%eax
	jmp L594
	.p2align 4,,7
L595:
	pushl %ebx
	call _FREELIST_INDEX__t24__default_alloc_template2b0i0Ui
	addl $4,%esp
	movl %eax,%eax
	leal 0(,%eax,4),%edx
	movl $__t24__default_alloc_template2b0i0$free_list,%eax
	leal (%edx,%eax),%esi
	movl %esi,-4(%ebp)
	movl -4(%ebp),%eax
	movl (%eax),%edx
	movl %edx,-8(%ebp)
	cmpl $0,-8(%ebp)
	jne L606
	pushl %ebx
	call _ROUND_UP__t24__default_alloc_template2b0i0Ui
	addl $4,%esp
	movl %eax,%eax
	pushl %eax
	call _refill__t24__default_alloc_template2b0i0Ui
	addl $4,%esp
	movl %eax,%eax
	movl %eax,-12(%ebp)
	movl -12(%ebp),%edx
	movl %edx,%eax
	jmp L594
	.p2align 4,,7
L606:
	movl -4(%ebp),%eax
	movl -8(%ebp),%edx
	movl (%edx),%ecx
	movl %ecx,(%eax)
	movl -8(%ebp),%edx
	movl %edx,%eax
	jmp L594
L594:
	leal -24(%ebp),%esp
	popl %ebx
	popl %esi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$allocate__t12simple_alloc2ZPiZt24__default_alloc_template2b0i0Ui,"x"
	.linkonce discard
	.align 4
.globl _allocate__t12simple_alloc2ZPiZt24__default_alloc_template2b0i0Ui
	.def	_allocate__t12simple_alloc2ZPiZt24__default_alloc_template2b0i0Ui;	.scl	2;	.type	32;	.endef
_allocate__t12simple_alloc2ZPiZt24__default_alloc_template2b0i0Ui:
	pushl %ebp
	movl %esp,%ebp
	pushl %ebx
	movl 8(%ebp),%ebx
	testl %ebx,%ebx
	je L627
	leal 0(,%ebx,4),%eax
	pushl %eax
	call _allocate__t24__default_alloc_template2b0i0Ui
	addl $4,%esp
	movl %eax,%eax
	jmp L628
	.p2align 4,,7
L627:
	xorl %eax,%eax
L628:
	movl %eax,%eax
	jmp L593
L593:
	movl -4(%ebp),%ebx
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$allocate__t12simple_alloc2ZiZt24__default_alloc_template2b0i0Ui,"x"
	.linkonce discard
	.align 4
.globl _allocate__t12simple_alloc2ZiZt24__default_alloc_template2b0i0Ui
	.def	_allocate__t12simple_alloc2ZiZt24__default_alloc_template2b0i0Ui;	.scl	2;	.type	32;	.endef
_allocate__t12simple_alloc2ZiZt24__default_alloc_template2b0i0Ui:
	pushl %ebp
	movl %esp,%ebp
	pushl %ebx
	movl 8(%ebp),%ebx
	testl %ebx,%ebx
	je L639
	leal 0(,%ebx,4),%eax
	pushl %eax
	call _allocate__t24__default_alloc_template2b0i0Ui
	addl $4,%esp
	movl %eax,%eax
	jmp L640
	.p2align 4,,7
L639:
	xorl %eax,%eax
L640:
	movl %eax,%eax
	jmp L638
L638:
	movl -4(%ebp),%ebx
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$allocate_node__t5deque3ZiZt24__default_alloc_template2b0i0Ui0,"x"
	.linkonce discard
	.align 4
.globl _allocate_node__t5deque3ZiZt24__default_alloc_template2b0i0Ui0
	.def	_allocate_node__t5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	2;	.type	32;	.endef
_allocate_node__t5deque3ZiZt24__default_alloc_template2b0i0Ui0:
	pushl %ebp
	movl %esp,%ebp
	pushl %ebx
	movl 8(%ebp),%ebx
	call _buffer_size__t5deque3ZiZt24__default_alloc_template2b0i0Ui0
	movl %eax,%eax
	pushl %eax
	call _allocate__t12simple_alloc2ZiZt24__default_alloc_template2b0i0Ui
	addl $4,%esp
	movl %eax,%edx
	movl %edx,%eax
	jmp L637
L637:
	movl -4(%ebp),%ebx
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$deallocate__t23__malloc_alloc_template1i0PvUi,"x"
	.linkonce discard
	.align 4
.globl _deallocate__t23__malloc_alloc_template1i0PvUi
	.def	_deallocate__t23__malloc_alloc_template1i0PvUi;	.scl	2;	.type	32;	.endef
_deallocate__t23__malloc_alloc_template1i0PvUi:
	pushl %ebp
	movl %esp,%ebp
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%esi
	movl 12(%ebp),%ebx
	pushl %esi
	call _free
	addl $4,%esp
L656:
	leal -8(%ebp),%esp
	popl %ebx
	popl %esi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$deallocate__t24__default_alloc_template2b0i0PvUi,"x"
	.linkonce discard
	.align 4
.globl _deallocate__t24__default_alloc_template2b0i0PvUi
	.def	_deallocate__t24__default_alloc_template2b0i0PvUi;	.scl	2;	.type	32;	.endef
_deallocate__t24__default_alloc_template2b0i0PvUi:
	pushl %ebp
	movl %esp,%ebp
	subl $16,%esp
	pushl %edi
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%ebx
	movl 12(%ebp),%esi
	movl %ebx,-4(%ebp)
	cmpl $128,%esi
	jbe L655
	pushl %esi
	pushl %ebx
	call _deallocate__t23__malloc_alloc_template1i0PvUi
	addl $8,%esp
	jmp L654
	.p2align 4,,7
L655:
	pushl %esi
	call _FREELIST_INDEX__t24__default_alloc_template2b0i0Ui
	addl $4,%esp
	movl %eax,%eax
	leal 0(,%eax,4),%edx
	movl $__t24__default_alloc_template2b0i0$free_list,%eax
	leal (%edx,%eax),%edi
	movl %edi,-8(%ebp)
	movl -4(%ebp),%eax
	movl -8(%ebp),%edx
	movl (%edx),%ecx
	movl %ecx,(%eax)
	movl -8(%ebp),%eax
	movl -4(%ebp),%edx
	movl %edx,(%eax)
L654:
	leal -28(%ebp),%esp
	popl %ebx
	popl %esi
	popl %edi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$deallocate__t12simple_alloc2ZiZt24__default_alloc_template2b0i0PiUi,"x"
	.linkonce discard
	.align 4
.globl _deallocate__t12simple_alloc2ZiZt24__default_alloc_template2b0i0PiUi
	.def	_deallocate__t12simple_alloc2ZiZt24__default_alloc_template2b0i0PiUi;	.scl	2;	.type	32;	.endef
_deallocate__t12simple_alloc2ZiZt24__default_alloc_template2b0i0PiUi:
	pushl %ebp
	movl %esp,%ebp
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%ebx
	movl 12(%ebp),%esi
	testl %esi,%esi
	je L653
	leal 0(,%esi,4),%eax
	pushl %eax
	pushl %ebx
	call _deallocate__t24__default_alloc_template2b0i0PvUi
	addl $8,%esp
L653:
L652:
	leal -8(%ebp),%esp
	popl %ebx
	popl %esi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$deallocate_node__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Pi,"x"
	.linkonce discard
	.align 4
.globl _deallocate_node__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Pi
	.def	_deallocate_node__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Pi;	.scl	2;	.type	32;	.endef
_deallocate_node__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Pi:
	pushl %ebp
	movl %esp,%ebp
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%ebx
	movl 12(%ebp),%esi
	call _buffer_size__t5deque3ZiZt24__default_alloc_template2b0i0Ui0
	movl %eax,%eax
	pushl %eax
	pushl %esi
	call _deallocate__t12simple_alloc2ZiZt24__default_alloc_template2b0i0PiUi
	addl $8,%esp
L651:
	leal -8(%ebp),%esp
	popl %ebx
	popl %esi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$deallocate__t12simple_alloc2ZPiZt24__default_alloc_template2b0i0PPiUi,"x"
	.linkonce discard
	.align 4
.globl _deallocate__t12simple_alloc2ZPiZt24__default_alloc_template2b0i0PPiUi
	.def	_deallocate__t12simple_alloc2ZPiZt24__default_alloc_template2b0i0PPiUi;	.scl	2;	.type	32;	.endef
_deallocate__t12simple_alloc2ZPiZt24__default_alloc_template2b0i0PPiUi:
	pushl %ebp
	movl %esp,%ebp
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%ebx
	movl 12(%ebp),%esi
	testl %esi,%esi
	je L658
	leal 0(,%esi,4),%eax
	pushl %eax
	pushl %ebx
	call _deallocate__t24__default_alloc_template2b0i0PvUi
	addl $8,%esp
L658:
L657:
	leal -8(%ebp),%esp
	popl %ebx
	popl %esi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$create_map_and_nodes__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Ui,"x"
	.linkonce discard
	.align 4
.globl _create_map_and_nodes__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Ui
	.def	_create_map_and_nodes__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Ui;	.scl	2;	.type	32;	.endef
_create_map_and_nodes__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Ui:
	pushl %ebp
	movl %esp,%ebp
	subl $144,%esp
	pushl %edi
	pushl %esi
	pushl %ebx
	call ___get_eh_context
	movl %eax,-132(%ebp)
	movl -132(%ebp),%edx
	movl %edx,-132(%ebp)
	movl -132(%ebp),%eax
	movl %eax,-128(%ebp)
	call _buffer_size__t5deque3ZiZt24__default_alloc_template2b0i0Ui0
	movl %eax,-132(%ebp)
	movl 12(%ebp),%ecx
	movl %ecx,%eax
	xorl %edx,%edx
	divl -132(%ebp)
	movl %eax,-132(%ebp)
	movl -132(%ebp),%ebx
	incl %ebx
	movl %ebx,-4(%ebp)
	movl -4(%ebp),%eax
	addl $2,%eax
	movl %eax,-8(%ebp)
	leal -8(%ebp),%ebx
	movl %ebx,-132(%ebp)
	movl -132(%ebp),%eax
	pushl %eax
	call _initial_map_size__t5deque3ZiZt24__default_alloc_template2b0i0Ui0
	movl %eax,-12(%ebp)
	leal -12(%ebp),%ebx
	movl %ebx,-132(%ebp)
	movl -132(%ebp),%eax
	pushl %eax
	call _max__H1ZUi_RCX01T0_RCX01
	addl $8,%esp
	movl %eax,-132(%ebp)
	movl -132(%ebp),%ebx
	movl %ebx,-124(%ebp)
	movl -124(%ebp),%eax
	movl (%eax),%eax
	movl %eax,-132(%ebp)
	movl -132(%ebp),%eax
	movl 8(%ebp),%ebx
	movl %eax,36(%ebx)
	movl 8(%ebp),%ebx
	movl 36(%ebx),%ebx
	movl %ebx,-132(%ebp)
	movl -132(%ebp),%eax
	pushl %eax
	call _allocate__t12simple_alloc2ZPiZt24__default_alloc_template2b0i0Ui
	addl $4,%esp
	movl %eax,-132(%ebp)
	movl -132(%ebp),%eax
	movl 8(%ebp),%ebx
	movl %eax,32(%ebx)
	movl 8(%ebp),%ebx
	movl 36(%ebx),%ebx
	movl %ebx,-132(%ebp)
	movl -4(%ebp),%edx
	movl -132(%ebp),%eax
	subl %edx,%eax
	movl %eax,-132(%ebp)
	movl -132(%ebp),%edx
	shrl $1,%edx
	leal 0(,%edx,4),%ebx
	movl %ebx,-132(%ebp)
	movl -132(%ebp),%ebx
	movl 8(%ebp),%eax
	addl 32(%eax),%ebx
	movl %ebx,-16(%ebp)
	movl -4(%ebp),%eax
	movl %eax,-132(%ebp)
	movl -132(%ebp),%edx
	leal 0(,%edx,4),%ebx
	movl %ebx,-132(%ebp)
	movl -132(%ebp),%edx
	addl -16(%ebp),%edx
	leal -4(%edx),%eax
	movl %eax,-20(%ebp)
	movl -128(%ebp),%ebx
	addl $4,%ebx
	movl %ebx,-132(%ebp)
	movl -132(%ebp),%eax
	movl (%eax),%edx
	movl %edx,-56(%ebp)
	movl $0,-52(%ebp)
	leal -48(%ebp),%edx
	movl %ebp,(%edx)
	movl $L632,4(%edx)
	movl %esp,8(%edx)
	xorl %edx,%edx
	jmp L631
	.p2align 4,,7
L632:
	movl %ebp,%ebp
	movl $1,%edx
	jmp L630
	.p2align 4,,7
L631:
	leal -56(%ebp),%eax
	movl -132(%ebp),%ebx
	movl %eax,(%ebx)
	movl -16(%ebp),%ebx
	movl %ebx,-132(%ebp)
	movl -132(%ebp),%eax
	movl %eax,-24(%ebp)
	.p2align 4,,7
L633:
	movl -24(%ebp),%ebx
	movl %ebx,-132(%ebp)
	movl -132(%ebp),%eax
	cmpl -20(%ebp),%eax
	jbe L636
	jmp L634
	.p2align 4,,7
L636:
	movl 8(%ebp),%ebx
	pushl %ebx
	call _allocate_node__t5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,-132(%ebp)
	movl -24(%ebp),%edx
	movl -132(%ebp),%eax
	movl %eax,(%edx)
L635:
	addl $4,-24(%ebp)
	jmp L633
	.p2align 4,,7
L634:
	movl -128(%ebp),%ebx
	addl $4,%ebx
	movl %ebx,-132(%ebp)
	movl -132(%ebp),%eax
	movl (%eax),%edx
	movl (%edx),%ecx
	movl -132(%ebp),%ebx
	movl %ecx,(%ebx)
L641:
	movl -16(%ebp),%eax
	movl %eax,-132(%ebp)
	movl -132(%ebp),%ebx
	pushl %ebx
	movl 8(%ebp),%eax
	pushl %eax
	call _set_node__t16__deque_iterator4ZiZRiZPiUi0PPi
	addl $8,%esp
	movl -20(%ebp),%ebx
	movl %ebx,-132(%ebp)
	movl -132(%ebp),%eax
	pushl %eax
	movl 8(%ebp),%ebx
	addl $16,%ebx
	movl %ebx,-132(%ebp)
	movl -132(%ebp),%eax
	pushl %eax
	call _set_node__t16__deque_iterator4ZiZRiZPiUi0PPi
	addl $8,%esp
	movl 8(%ebp),%ebx
	movl 4(%ebx),%ebx
	movl %ebx,-132(%ebp)
	movl -132(%ebp),%ebx
	movl 8(%ebp),%eax
	movl %ebx,(%eax)
	call _buffer_size__t5deque3ZiZt24__default_alloc_template2b0i0Ui0
	movl %eax,-132(%ebp)
	movl 12(%ebp),%ecx
	movl %ecx,%eax
	xorl %edx,%edx
	divl -132(%ebp)
	leal 0(,%edx,4),%ecx
	movl 8(%ebp),%ebx
	movl 20(%ebx),%eax
	addl %ecx,%eax
	movl %eax,16(%ebx)
	jmp L663
	.p2align 4,,7
L630:
	call ___cp_eh_info
	movl %eax,-132(%ebp)
	movl -132(%ebp),%ebx
	movl %ebx,-60(%ebp)
	movl -60(%ebp),%eax
	movl %eax,-132(%ebp)
	movl -132(%ebp),%ebx
	incl 28(%ebx)
	movl -128(%ebp),%eax
	addl $4,%eax
	movl %eax,-132(%ebp)
	movl -132(%ebp),%ebx
	movl (%ebx),%edx
	movl %edx,-88(%ebp)
	movl $0,-84(%ebp)
	leal -80(%ebp),%edx
	movl %ebp,(%edx)
	movl $L646,4(%edx)
	movl %esp,8(%edx)
	xorl %edx,%edx
	jmp L645
	.p2align 4,,7
L646:
	movl %ebp,%ebp
	movl $1,%edx
	jmp L644
	.p2align 4,,7
L645:
	leal -88(%ebp),%ebx
	movl -132(%ebp),%eax
	movl %ebx,(%eax)
	movl -60(%ebp),%eax
	movl %eax,-132(%ebp)
	movl -132(%ebp),%ebx
	movb $1,20(%ebx)
	movl -16(%ebp),%eax
	movl %eax,-132(%ebp)
	movl -132(%ebp),%ebx
	movl %ebx,-92(%ebp)
	.p2align 4,,7
L647:
	movl -92(%ebp),%eax
	movl %eax,-132(%ebp)
	movl -132(%ebp),%ebx
	cmpl -24(%ebp),%ebx
	jb L650
	jmp L648
	.p2align 4,,7
L650:
	movl -92(%ebp),%eax
	movl %eax,-132(%ebp)
	movl -132(%ebp),%ebx
	movl (%ebx),%edx
	pushl %edx
	movl 8(%ebp),%eax
	pushl %eax
	call _deallocate_node__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Pi
	addl $8,%esp
L649:
	addl $4,-92(%ebp)
	jmp L647
	.p2align 4,,7
L648:
	movl 8(%ebp),%ebx
	movl 36(%ebx),%ebx
	movl %ebx,-132(%ebp)
	movl -132(%ebp),%eax
	pushl %eax
	movl 8(%ebp),%ebx
	movl 32(%ebx),%ebx
	movl %ebx,-132(%ebp)
	movl -132(%ebp),%eax
	pushl %eax
	call _deallocate__t12simple_alloc2ZPiZt24__default_alloc_template2b0i0PPiUi
	addl $8,%esp
	call ___uncatch_exception
	call ___sjthrow
	movl -128(%ebp),%ebx
	addl $4,%ebx
	movl %ebx,-132(%ebp)
	movl -132(%ebp),%eax
	movl (%eax),%edx
	movl (%edx),%ecx
	movl -132(%ebp),%ebx
	movl %ecx,(%ebx)
	movl -60(%ebp),%eax
	movl %eax,-132(%ebp)
	movl -132(%ebp),%ebx
	pushl %ebx
	call ___cp_pop_exception
	addl $4,%esp
	jmp L641
	.p2align 4,,7
L642:
	call ___sjthrow
	.p2align 4,,7
L644:
	movl -128(%ebp),%eax
	addl $4,%eax
	movl %eax,-132(%ebp)
	movl -132(%ebp),%ebx
	movl (%ebx),%edx
	movl %edx,-120(%ebp)
	movl $0,-116(%ebp)
	leal -112(%ebp),%edx
	movl %ebp,(%edx)
	movl $L662,4(%edx)
	movl %esp,8(%edx)
	xorl %edx,%edx
	jmp L661
	.p2align 4,,7
L662:
	movl %ebp,%ebp
	movl $1,%edx
	jmp L660
	.p2align 4,,7
L661:
	leal -120(%ebp),%ebx
	movl -132(%ebp),%eax
	movl %ebx,(%eax)
	movl -60(%ebp),%eax
	movl %eax,-132(%ebp)
	movl -132(%ebp),%ebx
	pushl %ebx
	call ___cp_pop_exception
	addl $4,%esp
	movl -128(%ebp),%eax
	addl $4,%eax
	movl %eax,-132(%ebp)
	movl -132(%ebp),%ebx
	movl (%ebx),%edx
	movl (%edx),%ecx
	movl -132(%ebp),%eax
	movl %ecx,(%eax)
	call ___sjthrow
	.p2align 4,,7
L660:
	call ___terminate
	.p2align 4,,7
L663:
L584:
	leal -156(%ebp),%esp
	popl %ebx
	popl %esi
	popl %edi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$destroy_map_and_nodes__t5deque3ZiZt24__default_alloc_template2b0i0Ui0,"x"
	.linkonce discard
	.align 4
.globl _destroy_map_and_nodes__t5deque3ZiZt24__default_alloc_template2b0i0Ui0
	.def	_destroy_map_and_nodes__t5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	2;	.type	32;	.endef
_destroy_map_and_nodes__t5deque3ZiZt24__default_alloc_template2b0i0Ui0:
	pushl %ebp
	movl %esp,%ebp
	subl $16,%esp
	pushl %ebx
	movl 8(%ebp),%ebx
	movl 12(%ebx),%eax
	movl %eax,-4(%ebp)
	.p2align 4,,7
L665:
	movl -4(%ebp),%eax
	cmpl 28(%ebx),%eax
	jbe L668
	jmp L666
	.p2align 4,,7
L668:
	movl -4(%ebp),%eax
	movl (%eax),%edx
	pushl %edx
	pushl %ebx
	call _deallocate_node__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Pi
	addl $8,%esp
L667:
	addl $4,-4(%ebp)
	jmp L665
	.p2align 4,,7
L666:
	movl 36(%ebx),%eax
	pushl %eax
	movl 32(%ebx),%eax
	pushl %eax
	call _deallocate__t12simple_alloc2ZPiZt24__default_alloc_template2b0i0PPiUi
	addl $8,%esp
L664:
	movl -20(%ebp),%ebx
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$__copy_t__H1ZPi_PCX01T0PX01G11__true_type_PX01,"x"
	.linkonce discard
	.align 4
.globl ___copy_t__H1ZPi_PCX01T0PX01G11__true_type_PX01
	.def	___copy_t__H1ZPi_PCX01T0PX01G11__true_type_PX01;	.scl	2;	.type	32;	.endef
___copy_t__H1ZPi_PCX01T0PX01G11__true_type_PX01:
	pushl %ebp
	movl %esp,%ebp
	pushl %edi
	pushl %esi
	pushl %ebx
	movl 12(%ebp),%esi
	movl 16(%ebp),%edi
	movb 20(%ebp),%bl
	movl %esi,%eax
	subl 8(%ebp),%eax
	movl %eax,%edx
	movl %edx,%eax
	sarl $2,%eax
	leal 0(,%eax,4),%edx
	pushl %edx
	movl 8(%ebp),%ecx
	pushl %ecx
	pushl %edi
	call _memmove
	addl $12,%esp
	movl %esi,%eax
	subl 8(%ebp),%eax
	leal (%eax,%edi),%edx
	movl %edx,%eax
	jmp L678
L678:
	leal -12(%ebp),%esp
	popl %ebx
	popl %esi
	popl %edi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$__cl__t15__copy_dispatch2ZPPiZPPiPPiN21,"x"
	.linkonce discard
	.align 4
.globl ___cl__t15__copy_dispatch2ZPPiZPPiPPiN21
	.def	___cl__t15__copy_dispatch2ZPPiZPPiPPiN21;	.scl	2;	.type	32;	.endef
___cl__t15__copy_dispatch2ZPPiZPPiPPiN21:
	pushl %ebp
	movl %esp,%ebp
	pushl %edi
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%ebx
	movl 12(%ebp),%esi
	movl 16(%ebp),%edi
	movb $0,%al
	addl $-2,%esp
	pushw $0
	movl 20(%ebp),%ecx
	pushl %ecx
	pushl %edi
	pushl %esi
	call ___copy_t__H1ZPi_PCX01T0PX01G11__true_type_PX01
	addl $16,%esp
	movl %eax,%edx
	movl %edx,%eax
	jmp L677
L677:
	leal -12(%ebp),%esp
	popl %ebx
	popl %esi
	popl %edi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$copy__H2ZPPiZPPi_X01T0X11_X11,"x"
	.linkonce discard
	.align 4
.globl _copy__H2ZPPiZPPi_X01T0X11_X11
	.def	_copy__H2ZPPiZPPi_X01T0X11_X11;	.scl	2;	.type	32;	.endef
_copy__H2ZPPiZPPi_X01T0X11_X11:
	pushl %ebp
	movl %esp,%ebp
	subl $16,%esp
	pushl %edi
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%ebx
	movl 12(%ebp),%esi
	movl 16(%ebp),%edi
	pushl %edi
	pushl %esi
	pushl %ebx
	movb $0,-1(%ebp)
	leal -1(%ebp),%eax
	pushl %eax
	call ___cl__t15__copy_dispatch2ZPPiZPPiPPiN21
	addl $16,%esp
	movl %eax,%edx
	movl %edx,%eax
	jmp L676
L676:
	leal -28(%ebp),%esp
	popl %ebx
	popl %esi
	popl %edi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$__copy_backward_t__H1ZPi_PCX01T0PX01G11__true_type_PX01,"x"
	.linkonce discard
	.align 4
.globl ___copy_backward_t__H1ZPi_PCX01T0PX01G11__true_type_PX01
	.def	___copy_backward_t__H1ZPi_PCX01T0PX01G11__true_type_PX01;	.scl	2;	.type	32;	.endef
___copy_backward_t__H1ZPi_PCX01T0PX01G11__true_type_PX01:
	pushl %ebp
	movl %esp,%ebp
	subl $16,%esp
	pushl %edi
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%edi
	movl 12(%ebp),%esi
	movb 20(%ebp),%bl
	movl %esi,%ecx
	subl %edi,%ecx
	movl %ecx,-8(%ebp)
	movl -8(%ebp),%eax
	movl %eax,%edx
	sarl $2,%edx
	movl %edx,-8(%ebp)
	movl -8(%ebp),%ecx
	movl %ecx,-4(%ebp)
	movl -4(%ebp),%eax
	movl %eax,%edx
	leal 0(,%edx,4),%eax
	pushl %eax
	pushl %edi
	movl -4(%ebp),%eax
	movl %eax,%edx
	leal 0(,%edx,4),%eax
	movl 16(%ebp),%edx
	subl %eax,%edx
	pushl %edx
	call _memmove
	addl $12,%esp
	movl -4(%ebp),%eax
	movl %eax,%edx
	leal 0(,%edx,4),%eax
	movl 16(%ebp),%edx
	subl %eax,%edx
	movl %edx,%eax
	jmp L682
L682:
	leal -28(%ebp),%esp
	popl %ebx
	popl %esi
	popl %edi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$__cl__t24__copy_backward_dispatch2ZPPiZPPiPPiN21,"x"
	.linkonce discard
	.align 4
.globl ___cl__t24__copy_backward_dispatch2ZPPiZPPiPPiN21
	.def	___cl__t24__copy_backward_dispatch2ZPPiZPPiPPiN21;	.scl	2;	.type	32;	.endef
___cl__t24__copy_backward_dispatch2ZPPiZPPiPPiN21:
	pushl %ebp
	movl %esp,%ebp
	pushl %edi
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%ebx
	movl 12(%ebp),%esi
	movl 16(%ebp),%edi
	movb $0,%al
	addl $-2,%esp
	pushw $0
	movl 20(%ebp),%ecx
	pushl %ecx
	pushl %edi
	pushl %esi
	call ___copy_backward_t__H1ZPi_PCX01T0PX01G11__true_type_PX01
	addl $16,%esp
	movl %eax,%edx
	movl %edx,%eax
	jmp L681
L681:
	leal -12(%ebp),%esp
	popl %ebx
	popl %esi
	popl %edi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$copy_backward__H2ZPPiZPPi_X01T0X11_X11,"x"
	.linkonce discard
	.align 4
.globl _copy_backward__H2ZPPiZPPi_X01T0X11_X11
	.def	_copy_backward__H2ZPPiZPPi_X01T0X11_X11;	.scl	2;	.type	32;	.endef
_copy_backward__H2ZPPiZPPi_X01T0X11_X11:
	pushl %ebp
	movl %esp,%ebp
	subl $16,%esp
	pushl %edi
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%ebx
	movl 12(%ebp),%esi
	movl 16(%ebp),%edi
	pushl %edi
	pushl %esi
	pushl %ebx
	movb $0,-1(%ebp)
	leal -1(%ebp),%eax
	pushl %eax
	call ___cl__t24__copy_backward_dispatch2ZPPiZPPiPPiN21
	addl $16,%esp
	movl %eax,%edx
	movl %edx,%eax
	jmp L680
L680:
	leal -28(%ebp),%esp
	popl %ebx
	popl %esi
	popl %edi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$reallocate_map__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Uib,"x"
	.linkonce discard
	.align 4
.globl _reallocate_map__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Uib
	.def	_reallocate_map__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Uib;	.scl	2;	.type	32;	.endef
_reallocate_map__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Uib:
	pushl %ebp
	movl %esp,%ebp
	subl $48,%esp
	pushl %edi
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%edi
	movl 28(%edi),%eax
	movl 12(%edi),%edx
	subl %edx,%eax
	movl %eax,%edx
	movl %edx,%eax
	sarl $2,%eax
	leal 1(%eax),%ecx
	movl %ecx,-4(%ebp)
	movl -4(%ebp),%eax
	movl 12(%ebp),%edx
	leal (%edx,%eax),%ecx
	movl %ecx,-8(%ebp)
	movl -8(%ebp),%eax
	movl %eax,%edx
	movl %edx,%eax
	addl %edx,%eax
	cmpl %eax,36(%edi)
	jbe L673
	movl 36(%edi),%eax
	movl -8(%ebp),%edx
	subl %edx,%eax
	movl %eax,%edx
	shrl $1,%edx
	leal 0(,%edx,4),%eax
	movl %eax,%esi
	addl 32(%edi),%esi
	movl %esi,%ebx
	cmpb $0,16(%ebp)
	je L674
	movl 12(%ebp),%eax
	movl %eax,%edx
	leal 0(,%edx,4),%eax
	addl %eax,%ebx
L674:
	movl %ebx,-12(%ebp)
	movl -12(%ebp),%eax
	cmpl 12(%edi),%eax
	jae L675
	movl -12(%ebp),%eax
	pushl %eax
	movl 28(%edi),%eax
	addl $4,%eax
	pushl %eax
	movl 12(%edi),%eax
	pushl %eax
	call _copy__H2ZPPiZPPi_X01T0X11_X11
	addl $12,%esp
	jmp L679
	.p2align 4,,7
L675:
	movl -4(%ebp),%eax
	movl %eax,%edx
	leal 0(,%edx,4),%eax
	movl %eax,%edx
	addl -12(%ebp),%edx
	pushl %edx
	movl 28(%edi),%eax
	addl $4,%eax
	pushl %eax
	movl 12(%edi),%eax
	pushl %eax
	call _copy_backward__H2ZPPiZPPi_X01T0X11_X11
	addl $12,%esp
L679:
	jmp L683
	.p2align 4,,7
L673:
	leal 12(%ebp),%eax
	pushl %eax
	leal 36(%edi),%eax
	pushl %eax
	call _max__H1ZUi_RCX01T0_RCX01
	addl $8,%esp
	movl %eax,%eax
	movl 36(%edi),%edx
	movl (%eax),%ecx
	movl %ecx,-36(%ebp)
	movl -36(%ebp),%ecx
	leal (%ecx,%edx),%eax
	leal 2(%eax),%ecx
	movl %ecx,-16(%ebp)
	movl -16(%ebp),%eax
	pushl %eax
	call _allocate__t12simple_alloc2ZPiZt24__default_alloc_template2b0i0Ui
	addl $4,%esp
	movl %eax,%eax
	movl %eax,-20(%ebp)
	movl -16(%ebp),%eax
	movl -8(%ebp),%edx
	subl %edx,%eax
	movl %eax,%edx
	shrl $1,%edx
	leal 0(,%edx,4),%eax
	movl -20(%ebp),%ecx
	addl %eax,%ecx
	movl %ecx,-28(%ebp)
	movl -28(%ebp),%ecx
	movl %ecx,-24(%ebp)
	cmpb $0,16(%ebp)
	je L684
	movl 12(%ebp),%eax
	movl %eax,%edx
	leal 0(,%edx,4),%eax
	addl %eax,-24(%ebp)
L684:
	movl -24(%ebp),%ecx
	movl %ecx,-12(%ebp)
	movl -12(%ebp),%eax
	pushl %eax
	movl 28(%edi),%eax
	addl $4,%eax
	pushl %eax
	movl 12(%edi),%eax
	pushl %eax
	call _copy__H2ZPPiZPPi_X01T0X11_X11
	addl $12,%esp
	movl 36(%edi),%eax
	pushl %eax
	movl 32(%edi),%eax
	pushl %eax
	call _deallocate__t12simple_alloc2ZPiZt24__default_alloc_template2b0i0PPiUi
	addl $8,%esp
	movl -20(%ebp),%eax
	movl %eax,32(%edi)
	movl -16(%ebp),%eax
	movl %eax,36(%edi)
L683:
	movl -12(%ebp),%eax
	pushl %eax
	pushl %edi
	call _set_node__t16__deque_iterator4ZiZRiZPiUi0PPi
	addl $8,%esp
	movl -4(%ebp),%eax
	movl %eax,%edx
	leal 0(,%edx,4),%eax
	movl %eax,%edx
	addl -12(%ebp),%edx
	leal -4(%edx),%eax
	pushl %eax
	leal 16(%edi),%eax
	pushl %eax
	call _set_node__t16__deque_iterator4ZiZRiZPiUi0PPi
	addl $8,%esp
L672:
	leal -60(%ebp),%esp
	popl %ebx
	popl %esi
	popl %edi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$reserve_map_at_back__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Ui,"x"
	.linkonce discard
	.align 4
.globl _reserve_map_at_back__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Ui
	.def	_reserve_map_at_back__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Ui;	.scl	2;	.type	32;	.endef
_reserve_map_at_back__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Ui:
	pushl %ebp
	movl %esp,%ebp
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%esi
	movl 12(%ebp),%ebx
	leal 1(%ebx),%eax
	movl 28(%esi),%edx
	movl 32(%esi),%ecx
	subl %ecx,%edx
	movl %edx,%ecx
	movl %ecx,%edx
	sarl $2,%edx
	movl 36(%esi),%ecx
	subl %edx,%ecx
	cmpl %ecx,%eax
	jbe L671
	addl $-2,%esp
	pushw $0
	pushl %ebx
	pushl %esi
	call _reallocate_map__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Uib
	addl $12,%esp
L671:
L670:
	leal -8(%ebp),%esp
	popl %ebx
	popl %esi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$push_back_aux__t5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi,"x"
	.linkonce discard
	.align 4
.globl _push_back_aux__t5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi
	.def	_push_back_aux__t5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi;	.scl	2;	.type	32;	.endef
_push_back_aux__t5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi:
	pushl %ebp
	movl %esp,%ebp
	subl $112,%esp
	pushl %edi
	pushl %esi
	pushl %ebx
	call ___get_eh_context
	movl %eax,%eax
	movl %eax,%edx
	movl %edx,%eax
	movl %eax,-108(%ebp)
	movl 12(%ebp),%eax
	movl (%eax),%edx
	movl %edx,-4(%ebp)
	pushl $1
	movl 8(%ebp),%ebx
	pushl %ebx
	call _reserve_map_at_back__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Ui
	addl $8,%esp
	movl 8(%ebp),%ebx
	pushl %ebx
	call _allocate_node__t5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%eax
	movl 8(%ebp),%ebx
	movl 28(%ebx),%edx
	addl $4,%edx
	movl %eax,(%edx)
	movl -108(%ebp),%eax
	addl $4,%eax
	movl (%eax),%edx
	movl %edx,-40(%ebp)
	movl $0,-36(%ebp)
	leal -32(%ebp),%edx
	movl %ebp,(%edx)
	movl $L688,4(%edx)
	movl %esp,8(%edx)
	xorl %edx,%edx
	jmp L687
	.p2align 4,,7
L688:
	movl %ebp,%ebp
	movl $1,%edx
	jmp L686
	.p2align 4,,7
L687:
	leal -40(%ebp),%ebx
	movl %ebx,(%eax)
	leal -4(%ebp),%eax
	pushl %eax
	movl 8(%ebp),%ebx
	movl 16(%ebx),%eax
	pushl %eax
	call _construct__H2ZiZi_PX01RCX11_v
	addl $8,%esp
	movl 8(%ebp),%ebx
	movl 28(%ebx),%eax
	addl $4,%eax
	pushl %eax
	movl 8(%ebp),%eax
	addl $16,%eax
	pushl %eax
	call _set_node__t16__deque_iterator4ZiZRiZPiUi0PPi
	addl $8,%esp
	movl 8(%ebp),%ebx
	movl 20(%ebx),%eax
	movl 8(%ebp),%ebx
	movl %eax,16(%ebx)
	movl -108(%ebp),%eax
	addl $4,%eax
	movl (%eax),%edx
	movl (%edx),%ecx
	movl %ecx,(%eax)
L689:
	jmp L699
	.p2align 4,,7
L686:
	call ___cp_eh_info
	movl %eax,%eax
	movl %eax,-44(%ebp)
	movl -44(%ebp),%eax
	incl 28(%eax)
	movl -108(%ebp),%eax
	addl $4,%eax
	movl (%eax),%edx
	movl %edx,-72(%ebp)
	movl $0,-68(%ebp)
	leal -64(%ebp),%edx
	movl %ebp,(%edx)
	movl $L694,4(%edx)
	movl %esp,8(%edx)
	xorl %edx,%edx
	jmp L693
	.p2align 4,,7
L694:
	movl %ebp,%ebp
	movl $1,%edx
	jmp L692
	.p2align 4,,7
L693:
	leal -72(%ebp),%ebx
	movl %ebx,(%eax)
	movl -44(%ebp),%eax
	movb $1,20(%eax)
	movl 8(%ebp),%ebx
	movl 28(%ebx),%eax
	addl $4,%eax
	movl (%eax),%edx
	pushl %edx
	movl 8(%ebp),%ebx
	pushl %ebx
	call _deallocate_node__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Pi
	addl $8,%esp
	call ___uncatch_exception
	call ___sjthrow
	jmp L689
	.p2align 4,,7
L690:
	call ___sjthrow
	.p2align 4,,7
L692:
	movl -108(%ebp),%eax
	addl $4,%eax
	movl (%eax),%edx
	movl %edx,-104(%ebp)
	movl $0,-100(%ebp)
	leal -96(%ebp),%edx
	movl %ebp,(%edx)
	movl $L698,4(%edx)
	movl %esp,8(%edx)
	xorl %edx,%edx
	jmp L697
	.p2align 4,,7
L698:
	movl %ebp,%ebp
	movl $1,%edx
	jmp L696
	.p2align 4,,7
L697:
	leal -104(%ebp),%ebx
	movl %ebx,(%eax)
	movl -44(%ebp),%eax
	pushl %eax
	call ___cp_pop_exception
	addl $4,%esp
	movl -108(%ebp),%eax
	addl $4,%eax
	movl (%eax),%edx
	movl (%edx),%ecx
	movl %ecx,(%eax)
	call ___sjthrow
	.p2align 4,,7
L696:
	call ___terminate
	.p2align 4,,7
L699:
L669:
	leal -124(%ebp),%esp
	popl %ebx
	popl %esi
	popl %edi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$pop_back_aux__t5deque3ZiZt24__default_alloc_template2b0i0Ui0,"x"
	.linkonce discard
	.align 4
.globl _pop_back_aux__t5deque3ZiZt24__default_alloc_template2b0i0Ui0
	.def	_pop_back_aux__t5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	2;	.type	32;	.endef
_pop_back_aux__t5deque3ZiZt24__default_alloc_template2b0i0Ui0:
	pushl %ebp
	movl %esp,%ebp
	pushl %ebx
	movl 8(%ebp),%ebx
	movl 20(%ebx),%eax
	pushl %eax
	pushl %ebx
	call _deallocate_node__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Pi
	addl $8,%esp
	movl 28(%ebx),%eax
	addl $-4,%eax
	pushl %eax
	leal 16(%ebx),%eax
	pushl %eax
	call _set_node__t16__deque_iterator4ZiZRiZPiUi0PPi
	addl $8,%esp
	movl 24(%ebx),%edx
	addl $-4,%edx
	movl %edx,16(%ebx)
	movl 16(%ebx),%eax
	pushl %eax
	call _destroy__H1Zi_PX01_v
	addl $4,%esp
L700:
	movl -4(%ebp),%ebx
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$__copy_backward_t__H1Zi_PCX01T0PX01G11__true_type_PX01,"x"
	.linkonce discard
	.align 4
.globl ___copy_backward_t__H1Zi_PCX01T0PX01G11__true_type_PX01
	.def	___copy_backward_t__H1Zi_PCX01T0PX01G11__true_type_PX01;	.scl	2;	.type	32;	.endef
___copy_backward_t__H1Zi_PCX01T0PX01G11__true_type_PX01:
	pushl %ebp
	movl %esp,%ebp
	subl $16,%esp
	pushl %edi
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%edi
	movl 12(%ebp),%esi
	movb 20(%ebp),%bl
	movl %esi,%ecx
	subl %edi,%ecx
	movl %ecx,-8(%ebp)
	movl -8(%ebp),%eax
	movl %eax,%edx
	sarl $2,%edx
	movl %edx,-8(%ebp)
	movl -8(%ebp),%ecx
	movl %ecx,-4(%ebp)
	movl -4(%ebp),%eax
	movl %eax,%edx
	leal 0(,%edx,4),%eax
	pushl %eax
	pushl %edi
	movl -4(%ebp),%eax
	movl %eax,%edx
	leal 0(,%edx,4),%eax
	movl 16(%ebp),%edx
	subl %eax,%edx
	pushl %edx
	call _memmove
	addl $12,%esp
	movl -4(%ebp),%eax
	movl %eax,%edx
	leal 0(,%edx,4),%eax
	movl 16(%ebp),%edx
	subl %eax,%edx
	movl %edx,%eax
	jmp L705
L705:
	leal -28(%ebp),%esp
	popl %ebx
	popl %esi
	popl %edi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$__cl__t24__copy_backward_dispatch2ZPiZPiPiN21,"x"
	.linkonce discard
	.align 4
.globl ___cl__t24__copy_backward_dispatch2ZPiZPiPiN21
	.def	___cl__t24__copy_backward_dispatch2ZPiZPiPiN21;	.scl	2;	.type	32;	.endef
___cl__t24__copy_backward_dispatch2ZPiZPiPiN21:
	pushl %ebp
	movl %esp,%ebp
	pushl %edi
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%ebx
	movl 12(%ebp),%esi
	movl 16(%ebp),%edi
	movb $0,%al
	addl $-2,%esp
	pushw $0
	movl 20(%ebp),%ecx
	pushl %ecx
	pushl %edi
	pushl %esi
	call ___copy_backward_t__H1Zi_PCX01T0PX01G11__true_type_PX01
	addl $16,%esp
	movl %eax,%edx
	movl %edx,%eax
	jmp L704
L704:
	leal -12(%ebp),%esp
	popl %ebx
	popl %esi
	popl %edi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$copy_backward__H2ZPiZPi_X01T0X11_X11,"x"
	.linkonce discard
	.align 4
.globl _copy_backward__H2ZPiZPi_X01T0X11_X11
	.def	_copy_backward__H2ZPiZPi_X01T0X11_X11;	.scl	2;	.type	32;	.endef
_copy_backward__H2ZPiZPi_X01T0X11_X11:
	pushl %ebp
	movl %esp,%ebp
	subl $16,%esp
	pushl %edi
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%ebx
	movl 12(%ebp),%esi
	movl 16(%ebp),%edi
	pushl %edi
	pushl %esi
	pushl %ebx
	movb $0,-1(%ebp)
	leal -1(%ebp),%eax
	pushl %eax
	call ___cl__t24__copy_backward_dispatch2ZPiZPiPiN21
	addl $16,%esp
	movl %eax,%edx
	movl %edx,%eax
	jmp L703
L703:
	leal -28(%ebp),%esp
	popl %ebx
	popl %esi
	popl %edi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$value_type__H1ZPi_RCX01_PQ2t15iterator_traits1ZX0110value_type,"x"
	.linkonce discard
	.align 4
.globl _value_type__H1ZPi_RCX01_PQ2t15iterator_traits1ZX0110value_type
	.def	_value_type__H1ZPi_RCX01_PQ2t15iterator_traits1ZX0110value_type;	.scl	2;	.type	32;	.endef
_value_type__H1ZPi_RCX01_PQ2t15iterator_traits1ZX0110value_type:
	pushl %ebp
	movl %esp,%ebp
	movl 8(%ebp),%edx
	xorl %eax,%eax
	jmp L714
L714:
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$__copy_t__H1Zi_PCX01T0PX01G11__true_type_PX01,"x"
	.linkonce discard
	.align 4
.globl ___copy_t__H1Zi_PCX01T0PX01G11__true_type_PX01
	.def	___copy_t__H1Zi_PCX01T0PX01G11__true_type_PX01;	.scl	2;	.type	32;	.endef
___copy_t__H1Zi_PCX01T0PX01G11__true_type_PX01:
	pushl %ebp
	movl %esp,%ebp
	pushl %edi
	pushl %esi
	pushl %ebx
	movl 12(%ebp),%esi
	movl 16(%ebp),%edi
	movb 20(%ebp),%bl
	movl %esi,%eax
	subl 8(%ebp),%eax
	movl %eax,%edx
	movl %edx,%eax
	sarl $2,%eax
	leal 0(,%eax,4),%edx
	pushl %edx
	movl 8(%ebp),%ecx
	pushl %ecx
	pushl %edi
	call _memmove
	addl $12,%esp
	movl %esi,%eax
	subl 8(%ebp),%eax
	leal (%eax,%edi),%edx
	movl %edx,%eax
	jmp L719
L719:
	leal -12(%ebp),%esp
	popl %ebx
	popl %esi
	popl %edi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$__cl__t15__copy_dispatch2ZPiZPiPiN21,"x"
	.linkonce discard
	.align 4
.globl ___cl__t15__copy_dispatch2ZPiZPiPiN21
	.def	___cl__t15__copy_dispatch2ZPiZPiPiN21;	.scl	2;	.type	32;	.endef
___cl__t15__copy_dispatch2ZPiZPiPiN21:
	pushl %ebp
	movl %esp,%ebp
	pushl %edi
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%ebx
	movl 12(%ebp),%esi
	movl 16(%ebp),%edi
	movb $0,%al
	addl $-2,%esp
	pushw $0
	movl 20(%ebp),%ecx
	pushl %ecx
	pushl %edi
	pushl %esi
	call ___copy_t__H1Zi_PCX01T0PX01G11__true_type_PX01
	addl $16,%esp
	movl %eax,%edx
	movl %edx,%eax
	jmp L718
L718:
	leal -12(%ebp),%esp
	popl %ebx
	popl %esi
	popl %edi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$copy__H2ZPiZPi_X01T0X11_X11,"x"
	.linkonce discard
	.align 4
.globl _copy__H2ZPiZPi_X01T0X11_X11
	.def	_copy__H2ZPiZPi_X01T0X11_X11;	.scl	2;	.type	32;	.endef
_copy__H2ZPiZPi_X01T0X11_X11:
	pushl %ebp
	movl %esp,%ebp
	subl $16,%esp
	pushl %edi
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%ebx
	movl 12(%ebp),%esi
	movl 16(%ebp),%edi
	pushl %edi
	pushl %esi
	pushl %ebx
	movb $0,-1(%ebp)
	leal -1(%ebp),%eax
	pushl %eax
	call ___cl__t15__copy_dispatch2ZPiZPiPiN21
	addl $16,%esp
	movl %eax,%edx
	movl %edx,%eax
	jmp L717
L717:
	leal -28(%ebp),%esp
	popl %ebx
	popl %esi
	popl %edi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$__uninitialized_copy_aux__H2ZPiZPi_X01T0X11G11__true_type_X11,"x"
	.linkonce discard
	.align 4
.globl ___uninitialized_copy_aux__H2ZPiZPi_X01T0X11G11__true_type_X11
	.def	___uninitialized_copy_aux__H2ZPiZPi_X01T0X11G11__true_type_X11;	.scl	2;	.type	32;	.endef
___uninitialized_copy_aux__H2ZPiZPi_X01T0X11G11__true_type_X11:
	pushl %ebp
	movl %esp,%ebp
	pushl %edi
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%esi
	movl 12(%ebp),%edi
	movb 20(%ebp),%bl
	movl 16(%ebp),%ecx
	pushl %ecx
	pushl %edi
	pushl %esi
	call _copy__H2ZPiZPi_X01T0X11_X11
	addl $12,%esp
	movl %eax,%edx
	movl %edx,%eax
	jmp L716
L716:
	leal -12(%ebp),%esp
	popl %ebx
	popl %esi
	popl %edi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$__uninitialized_copy__H3ZPiZPiZi_X01T0X11PX21_X11,"x"
	.linkonce discard
	.align 4
.globl ___uninitialized_copy__H3ZPiZPiZi_X01T0X11PX21_X11
	.def	___uninitialized_copy__H3ZPiZPiZi_X01T0X11PX21_X11;	.scl	2;	.type	32;	.endef
___uninitialized_copy__H3ZPiZPiZi_X01T0X11PX21_X11:
	pushl %ebp
	movl %esp,%ebp
	pushl %edi
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%esi
	movl 12(%ebp),%edi
	movl 20(%ebp),%ebx
	movb $0,%al
	addl $-2,%esp
	pushw $0
	movl 16(%ebp),%ecx
	pushl %ecx
	pushl %edi
	pushl %esi
	call ___uninitialized_copy_aux__H2ZPiZPi_X01T0X11G11__true_type_X11
	addl $16,%esp
	movl %eax,%edx
	movl %edx,%eax
	jmp L715
L715:
	leal -12(%ebp),%esp
	popl %ebx
	popl %esi
	popl %edi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$uninitialized_copy__H2ZPiZPi_X01T0X11_X11,"x"
	.linkonce discard
	.align 4
.globl _uninitialized_copy__H2ZPiZPi_X01T0X11_X11
	.def	_uninitialized_copy__H2ZPiZPi_X01T0X11_X11;	.scl	2;	.type	32;	.endef
_uninitialized_copy__H2ZPiZPi_X01T0X11_X11:
	pushl %ebp
	movl %esp,%ebp
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%ebx
	movl 12(%ebp),%esi
	movl 16(%ebp),%eax
	leal 16(%ebp),%eax
	pushl %eax
	call _value_type__H1ZPi_RCX01_PQ2t15iterator_traits1ZX0110value_type
	addl $4,%esp
	movl %eax,%eax
	pushl %eax
	movl 16(%ebp),%eax
	pushl %eax
	pushl %esi
	pushl %ebx
	call ___uninitialized_copy__H3ZPiZPiZi_X01T0X11PX21_X11
	addl $16,%esp
	movl %eax,%edx
	movl %edx,%eax
	jmp L713
L713:
	leal -8(%ebp),%esp
	popl %ebx
	popl %esi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$__destroy_aux__H1ZPi_X01T0G11__true_type_v,"x"
	.linkonce discard
	.align 4
.globl ___destroy_aux__H1ZPi_X01T0G11__true_type_v
	.def	___destroy_aux__H1ZPi_X01T0G11__true_type_v;	.scl	2;	.type	32;	.endef
___destroy_aux__H1ZPi_X01T0G11__true_type_v:
	pushl %ebp
	movl %esp,%ebp
	movl 8(%ebp),%eax
	movl 12(%ebp),%edx
	movb 16(%ebp),%cl
L728:
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$__destroy__H2ZPiZi_X01T0PX11_v,"x"
	.linkonce discard
	.align 4
.globl ___destroy__H2ZPiZi_X01T0PX11_v
	.def	___destroy__H2ZPiZi_X01T0PX11_v;	.scl	2;	.type	32;	.endef
___destroy__H2ZPiZi_X01T0PX11_v:
	pushl %ebp
	movl %esp,%ebp
	pushl %edi
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%esi
	movl 12(%ebp),%edi
	movl 16(%ebp),%ebx
	movb $0,%al
	addl $-2,%esp
	pushw $0
	pushl %edi
	pushl %esi
	call ___destroy_aux__H1ZPi_X01T0G11__true_type_v
	addl $12,%esp
L727:
	leal -12(%ebp),%esp
	popl %ebx
	popl %esi
	popl %edi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$destroy__H1ZPi_X01T0_v,"x"
	.linkonce discard
	.align 4
.globl _destroy__H1ZPi_X01T0_v
	.def	_destroy__H1ZPi_X01T0_v;	.scl	2;	.type	32;	.endef
_destroy__H1ZPi_X01T0_v:
	pushl %ebp
	movl %esp,%ebp
	pushl %ebx
	movl 12(%ebp),%ebx
	movl 8(%ebp),%eax
	leal 8(%ebp),%edx
	pushl %edx
	call _value_type__H1ZPi_RCX01_PQ2t15iterator_traits1ZX0110value_type
	addl $4,%esp
	movl %eax,%eax
	pushl %eax
	pushl %ebx
	movl 8(%ebp),%eax
	pushl %eax
	call ___destroy__H2ZPiZi_X01T0PX11_v
	addl $12,%esp
L726:
	movl -4(%ebp),%ebx
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$deallocate__t6vector2ZiZt24__default_alloc_template2b0i0,"x"
	.linkonce discard
	.align 4
.globl _deallocate__t6vector2ZiZt24__default_alloc_template2b0i0
	.def	_deallocate__t6vector2ZiZt24__default_alloc_template2b0i0;	.scl	2;	.type	32;	.endef
_deallocate__t6vector2ZiZt24__default_alloc_template2b0i0:
	pushl %ebp
	movl %esp,%ebp
	pushl %ebx
	movl 8(%ebp),%ebx
	cmpl $0,(%ebx)
	je L730
	movl 8(%ebx),%eax
	movl (%ebx),%edx
	subl %edx,%eax
	movl %eax,%edx
	movl %edx,%eax
	sarl $2,%eax
	pushl %eax
	movl (%ebx),%eax
	pushl %eax
	call _deallocate__t12simple_alloc2ZiZt24__default_alloc_template2b0i0PiUi
	addl $8,%esp
L730:
L729:
	movl -4(%ebp),%ebx
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$insert_aux__t6vector2ZiZt24__default_alloc_template2b0i0PiRCi,"x"
	.linkonce discard
	.align 4
.globl _insert_aux__t6vector2ZiZt24__default_alloc_template2b0i0PiRCi
	.def	_insert_aux__t6vector2ZiZt24__default_alloc_template2b0i0PiRCi;	.scl	2;	.type	32;	.endef
_insert_aux__t6vector2ZiZt24__default_alloc_template2b0i0PiRCi:
	pushl %ebp
	movl %esp,%ebp
	subl $128,%esp
	pushl %edi
	pushl %esi
	pushl %ebx
	call ___get_eh_context
	movl %eax,%eax
	movl %eax,%edx
	movl %edx,%eax
	movl %eax,-116(%ebp)
	movl 8(%ebp),%ebx
	movl 4(%ebx),%eax
	movl 8(%ebp),%esi
	cmpl 8(%esi),%eax
	je L702
	movl 8(%ebp),%ebx
	movl 4(%ebx),%eax
	addl $-4,%eax
	pushl %eax
	movl 8(%ebp),%esi
	movl 4(%esi),%eax
	pushl %eax
	call _construct__H2ZiZi_PX01RCX11_v
	addl $8,%esp
	movl 8(%ebp),%ebx
	addl $4,4(%ebx)
	movl 16(%ebp),%eax
	movl (%eax),%edx
	movl %edx,-4(%ebp)
	movl 8(%ebp),%esi
	movl 4(%esi),%eax
	addl $-4,%eax
	pushl %eax
	movl 8(%ebp),%ebx
	movl 4(%ebx),%eax
	addl $-8,%eax
	pushl %eax
	movl 12(%ebp),%eax
	pushl %eax
	call _copy_backward__H2ZPiZPi_X01T0X11_X11
	addl $12,%esp
	movl 12(%ebp),%eax
	movl -4(%ebp),%edx
	movl %edx,(%eax)
	jmp L706
	.p2align 4,,7
L702:
	movl 8(%ebp),%esi
	pushl %esi
	call _size__Ct6vector2ZiZt24__default_alloc_template2b0i0
	addl $4,%esp
	movl %eax,-108(%ebp)
	movl -108(%ebp),%ebx
	movl %ebx,-4(%ebp)
	cmpl $0,-4(%ebp)
	je L707
	movl -4(%ebp),%eax
	movl %eax,%edx
	movl %edx,%eax
	addl %edx,%eax
	movl %eax,-112(%ebp)
	jmp L708
	.p2align 4,,7
L707:
	movl $1,-112(%ebp)
L708:
	movl -112(%ebp),%esi
	movl %esi,-8(%ebp)
	movl -8(%ebp),%eax
	pushl %eax
	call _allocate__t12simple_alloc2ZiZt24__default_alloc_template2b0i0Ui
	addl $4,%esp
	movl %eax,%eax
	movl %eax,-12(%ebp)
	movl -12(%ebp),%eax
	movl %eax,-16(%ebp)
	movl -116(%ebp),%eax
	addl $4,%eax
	movl (%eax),%edx
	movl %edx,-40(%ebp)
	movl $0,-36(%ebp)
	leal -32(%ebp),%edx
	movl %ebp,(%edx)
	movl $L712,4(%edx)
	movl %esp,8(%edx)
	xorl %edx,%edx
	jmp L711
	.p2align 4,,7
L712:
	movl %ebp,%ebp
	movl $1,%edx
	jmp L710
	.p2align 4,,7
L711:
	leal -40(%ebp),%ebx
	movl %ebx,(%eax)
	movl -12(%ebp),%eax
	pushl %eax
	movl 12(%ebp),%eax
	pushl %eax
	movl 8(%ebp),%esi
	movl (%esi),%eax
	pushl %eax
	call _uninitialized_copy__H2ZPiZPi_X01T0X11_X11
	addl $12,%esp
	movl %eax,%eax
	movl %eax,-16(%ebp)
	movl 16(%ebp),%eax
	pushl %eax
	movl -16(%ebp),%eax
	pushl %eax
	call _construct__H2ZiZi_PX01RCX11_v
	addl $8,%esp
	addl $4,-16(%ebp)
	movl -16(%ebp),%eax
	pushl %eax
	movl 8(%ebp),%ebx
	movl 4(%ebx),%eax
	pushl %eax
	movl 12(%ebp),%eax
	pushl %eax
	call _uninitialized_copy__H2ZPiZPi_X01T0X11_X11
	addl $12,%esp
	movl %eax,%eax
	movl %eax,-16(%ebp)
	movl -116(%ebp),%eax
	addl $4,%eax
	movl (%eax),%edx
	movl (%edx),%ecx
	movl %ecx,(%eax)
L720:
	movl 8(%ebp),%esi
	pushl %esi
	call _end__t6vector2ZiZt24__default_alloc_template2b0i0
	addl $4,%esp
	movl %eax,%eax
	pushl %eax
	movl 8(%ebp),%ebx
	pushl %ebx
	call _begin__t6vector2ZiZt24__default_alloc_template2b0i0
	addl $4,%esp
	movl %eax,%eax
	pushl %eax
	call _destroy__H1ZPi_X01T0_v
	addl $8,%esp
	movl 8(%ebp),%esi
	pushl %esi
	call _deallocate__t6vector2ZiZt24__default_alloc_template2b0i0
	addl $4,%esp
	movl -12(%ebp),%eax
	movl 8(%ebp),%ebx
	movl %eax,(%ebx)
	movl -16(%ebp),%eax
	movl 8(%ebp),%esi
	movl %eax,4(%esi)
	movl -8(%ebp),%eax
	movl %eax,%edx
	leal 0(,%edx,4),%eax
	movl -12(%ebp),%esi
	addl %eax,%esi
	movl 8(%ebp),%ebx
	movl %esi,8(%ebx)
L706:
	jmp L735
	.p2align 4,,7
L710:
	call ___cp_eh_info
	movl %eax,%eax
	movl %eax,-44(%ebp)
	movl -44(%ebp),%eax
	incl 28(%eax)
	movl -116(%ebp),%eax
	addl $4,%eax
	movl (%eax),%edx
	movl %edx,-72(%ebp)
	movl $0,-68(%ebp)
	leal -64(%ebp),%edx
	movl %ebp,(%edx)
	movl $L725,4(%edx)
	movl %esp,8(%edx)
	xorl %edx,%edx
	jmp L724
	.p2align 4,,7
L725:
	movl %ebp,%ebp
	movl $1,%edx
	jmp L723
	.p2align 4,,7
L724:
	leal -72(%ebp),%ebx
	movl %ebx,(%eax)
	movl -44(%ebp),%eax
	movb $1,20(%eax)
	movl -16(%ebp),%eax
	pushl %eax
	movl -12(%ebp),%eax
	pushl %eax
	call _destroy__H1ZPi_X01T0_v
	addl $8,%esp
	movl -8(%ebp),%eax
	pushl %eax
	movl -12(%ebp),%eax
	pushl %eax
	call _deallocate__t12simple_alloc2ZiZt24__default_alloc_template2b0i0PiUi
	addl $8,%esp
	call ___uncatch_exception
	call ___sjthrow
	jmp L720
	.p2align 4,,7
L721:
	call ___sjthrow
	.p2align 4,,7
L723:
	movl -116(%ebp),%eax
	addl $4,%eax
	movl (%eax),%edx
	movl %edx,-104(%ebp)
	movl $0,-100(%ebp)
	leal -96(%ebp),%edx
	movl %ebp,(%edx)
	movl $L734,4(%edx)
	movl %esp,8(%edx)
	xorl %edx,%edx
	jmp L733
	.p2align 4,,7
L734:
	movl %ebp,%ebp
	movl $1,%edx
	jmp L732
	.p2align 4,,7
L733:
	leal -104(%ebp),%esi
	movl %esi,(%eax)
	movl -44(%ebp),%eax
	pushl %eax
	call ___cp_pop_exception
	addl $4,%esp
	movl -116(%ebp),%eax
	addl $4,%eax
	movl (%eax),%edx
	movl (%edx),%ecx
	movl %ecx,(%eax)
	call ___sjthrow
	.p2align 4,,7
L732:
	call ___terminate
	.p2align 4,,7
L735:
L701:
	leal -140(%ebp),%esp
	popl %ebx
	popl %esi
	popl %edi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$_$_t6vector2ZiZt24__default_alloc_template2b0i0,"x"
	.linkonce discard
	.align 4
.globl __$_t6vector2ZiZt24__default_alloc_template2b0i0
	.def	__$_t6vector2ZiZt24__default_alloc_template2b0i0;	.scl	2;	.type	32;	.endef
__$_t6vector2ZiZt24__default_alloc_template2b0i0:
	pushl %ebp
	movl %esp,%ebp
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%esi
	movl 12(%ebp),%ebx
	movl 4(%esi),%eax
	pushl %eax
	movl (%esi),%eax
	pushl %eax
	call _destroy__H1ZPi_X01T0_v
	addl $8,%esp
	pushl %esi
	call _deallocate__t6vector2ZiZt24__default_alloc_template2b0i0
	addl $4,%esp
L738:
	movl %ebx,%eax
	andl $1,%eax
	testl %eax,%eax
	je L740
	pushl %esi
	call ___builtin_delete
	addl $4,%esp
	jmp L740
	.p2align 4,,7
L739:
L740:
L737:
	leal -8(%ebp),%esp
	popl %ebx
	popl %esi
	movl %ebp,%esp
	popl %ebp
	ret
.text
	.align 4
.globl __GLOBAL_$D$strs
	.def	__GLOBAL_$D$strs;	.scl	2;	.type	32;	.endef
__GLOBAL_$D$strs:
	pushl %ebp
	movl %esp,%ebp
	pushl $2
	pushl $_heap
	call __$_t6vector2ZiZt24__default_alloc_template2b0i0
	addl $8,%esp
L736:
	movl %ebp,%esp
	popl %ebp
	ret
	.section .dtor
	.long	__GLOBAL_$D$strs
.section	.text$__t6vector2ZiZt24__default_alloc_template2b0i0,"x"
	.linkonce discard
	.align 4
.globl ___t6vector2ZiZt24__default_alloc_template2b0i0
	.def	___t6vector2ZiZt24__default_alloc_template2b0i0;	.scl	2;	.type	32;	.endef
___t6vector2ZiZt24__default_alloc_template2b0i0:
	pushl %ebp
	movl %esp,%ebp
	movl 8(%ebp),%edx
	movl $0,(%edx)
	movl $0,4(%edx)
	movl $0,8(%edx)
L743:
	movl %edx,%eax
	jmp L742
L742:
	movl %ebp,%esp
	popl %ebp
	ret
.text
	.align 4
.globl __GLOBAL_$I$strs
	.def	__GLOBAL_$I$strs;	.scl	2;	.type	32;	.endef
__GLOBAL_$I$strs:
	pushl %ebp
	movl %esp,%ebp
	movl _vars,%eax
	movl %eax,_instr
	movl _vars+4,%eax
	movl %eax,_value
	movl _vars+12,%eax
	movl %eax,_input
	movl _vars+16,%eax
	movl %eax,_output
	pushl $_heap
	call ___t6vector2ZiZt24__default_alloc_template2b0i0
	addl $4,%esp
L741:
	movl %ebp,%esp
	popl %ebp
	ret
	.section .ctor
	.long	__GLOBAL_$I$strs
.section	.text$__vc__t6vector2ZiZt24__default_alloc_template2b0i0Ui,"x"
	.linkonce discard
	.align 4
.globl ___vc__t6vector2ZiZt24__default_alloc_template2b0i0Ui
	.def	___vc__t6vector2ZiZt24__default_alloc_template2b0i0Ui;	.scl	2;	.type	32;	.endef
___vc__t6vector2ZiZt24__default_alloc_template2b0i0Ui:
	pushl %ebp
	movl %esp,%ebp
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%ebx
	movl 12(%ebp),%esi
	pushl %ebx
	call _begin__t6vector2ZiZt24__default_alloc_template2b0i0
	addl $4,%esp
	movl %eax,%eax
	leal 0(,%esi,4),%ecx
	leal (%ecx,%eax),%edx
	movl %edx,%eax
	jmp L543
	jmp L546
	jmp L543
	.p2align 4,,7
L546:
L543:
	leal -8(%ebp),%esp
	popl %ebx
	popl %esi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$begin__t6vector2ZiZt24__default_alloc_template2b0i0,"x"
	.linkonce discard
	.align 4
.globl _begin__t6vector2ZiZt24__default_alloc_template2b0i0
	.def	_begin__t6vector2ZiZt24__default_alloc_template2b0i0;	.scl	2;	.type	32;	.endef
_begin__t6vector2ZiZt24__default_alloc_template2b0i0:
	pushl %ebp
	movl %esp,%ebp
	movl 8(%ebp),%edx
	movl (%edx),%ecx
	movl %ecx,%eax
	jmp L544
	jmp L545
	jmp L544
	.p2align 4,,7
L545:
L544:
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$push_back__t6vector2ZiZt24__default_alloc_template2b0i0RCi,"x"
	.linkonce discard
	.align 4
.globl _push_back__t6vector2ZiZt24__default_alloc_template2b0i0RCi
	.def	_push_back__t6vector2ZiZt24__default_alloc_template2b0i0RCi;	.scl	2;	.type	32;	.endef
_push_back__t6vector2ZiZt24__default_alloc_template2b0i0RCi:
	pushl %ebp
	movl %esp,%ebp
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%esi
	movl 12(%ebp),%ebx
	movl 4(%esi),%eax
	cmpl 8(%esi),%eax
	je L537
	pushl %ebx
	movl 4(%esi),%eax
	pushl %eax
	call _construct__H2ZiZi_PX01RCX11_v
	addl $8,%esp
	addl $4,4(%esi)
	jmp L538
	.p2align 4,,7
L537:
	pushl %ebx
	pushl %esi
	call _end__t6vector2ZiZt24__default_alloc_template2b0i0
	addl $4,%esp
	movl %eax,%eax
	pushl %eax
	pushl %esi
	call _insert_aux__t6vector2ZiZt24__default_alloc_template2b0i0PiRCi
	addl $12,%esp
L538:
	jmp L541
	jmp L536
	.p2align 4,,7
L541:
L536:
	leal -8(%ebp),%esp
	popl %ebx
	popl %esi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$end__t6vector2ZiZt24__default_alloc_template2b0i0,"x"
	.linkonce discard
	.align 4
.globl _end__t6vector2ZiZt24__default_alloc_template2b0i0
	.def	_end__t6vector2ZiZt24__default_alloc_template2b0i0;	.scl	2;	.type	32;	.endef
_end__t6vector2ZiZt24__default_alloc_template2b0i0:
	pushl %ebp
	movl %esp,%ebp
	movl 8(%ebp),%edx
	movl 4(%edx),%ecx
	movl %ecx,%eax
	jmp L539
	jmp L540
	jmp L539
	.p2align 4,,7
L540:
L539:
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$size__Ct6vector2ZiZt24__default_alloc_template2b0i0,"x"
	.linkonce discard
	.align 4
.globl _size__Ct6vector2ZiZt24__default_alloc_template2b0i0
	.def	_size__Ct6vector2ZiZt24__default_alloc_template2b0i0;	.scl	2;	.type	32;	.endef
_size__Ct6vector2ZiZt24__default_alloc_template2b0i0:
	pushl %ebp
	movl %esp,%ebp
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%ebx
	pushl %ebx
	call _end__Ct6vector2ZiZt24__default_alloc_template2b0i0
	addl $4,%esp
	movl %eax,%esi
	pushl %ebx
	call _begin__Ct6vector2ZiZt24__default_alloc_template2b0i0
	addl $4,%esp
	movl %eax,%edx
	movl %esi,%eax
	subl %edx,%eax
	movl %eax,%edx
	movl %edx,%ecx
	sarl $2,%ecx
	movl %ecx,%eax
	jmp L530
	jmp L535
	jmp L530
	.p2align 4,,7
L535:
L530:
	leal -8(%ebp),%esp
	popl %ebx
	popl %esi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$end__Ct6vector2ZiZt24__default_alloc_template2b0i0,"x"
	.linkonce discard
	.align 4
.globl _end__Ct6vector2ZiZt24__default_alloc_template2b0i0
	.def	_end__Ct6vector2ZiZt24__default_alloc_template2b0i0;	.scl	2;	.type	32;	.endef
_end__Ct6vector2ZiZt24__default_alloc_template2b0i0:
	pushl %ebp
	movl %esp,%ebp
	movl 8(%ebp),%edx
	movl 4(%edx),%ecx
	movl %ecx,%eax
	jmp L533
	jmp L534
	jmp L533
	.p2align 4,,7
L534:
L533:
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$begin__Ct6vector2ZiZt24__default_alloc_template2b0i0,"x"
	.linkonce discard
	.align 4
.globl _begin__Ct6vector2ZiZt24__default_alloc_template2b0i0
	.def	_begin__Ct6vector2ZiZt24__default_alloc_template2b0i0;	.scl	2;	.type	32;	.endef
_begin__Ct6vector2ZiZt24__default_alloc_template2b0i0:
	pushl %ebp
	movl %esp,%ebp
	movl 8(%ebp),%edx
	movl (%edx),%ecx
	movl %ecx,%eax
	jmp L531
	jmp L532
	jmp L531
	.p2align 4,,7
L532:
L531:
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0,"x"
	.linkonce discard
	.align 4
.globl _pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	.def	_pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	2;	.type	32;	.endef
_pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0:
	pushl %ebp
	movl %esp,%ebp
	pushl %ebx
	movl 8(%ebp),%ebx
	pushl %ebx
	call _pop_back__t5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
L513:
	movl -4(%ebp),%ebx
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$pop_back__t5deque3ZiZt24__default_alloc_template2b0i0Ui0,"x"
	.linkonce discard
	.align 4
.globl _pop_back__t5deque3ZiZt24__default_alloc_template2b0i0Ui0
	.def	_pop_back__t5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	2;	.type	32;	.endef
_pop_back__t5deque3ZiZt24__default_alloc_template2b0i0Ui0:
	pushl %ebp
	movl %esp,%ebp
	pushl %ebx
	movl 8(%ebp),%ebx
	movl 16(%ebx),%eax
	cmpl 20(%ebx),%eax
	je L515
	addl $-4,16(%ebx)
	movl 16(%ebx),%eax
	pushl %eax
	call _destroy__H1Zi_PX01_v
	addl $4,%esp
	jmp L517
	.p2align 4,,7
L515:
	pushl %ebx
	call _pop_back_aux__t5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
L517:
L514:
	movl -4(%ebp),%ebx
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$destroy__H1Zi_PX01_v,"x"
	.linkonce discard
	.align 4
.globl _destroy__H1Zi_PX01_v
	.def	_destroy__H1Zi_PX01_v;	.scl	2;	.type	32;	.endef
_destroy__H1Zi_PX01_v:
	pushl %ebp
	movl %esp,%ebp
	movl 8(%ebp),%eax
L516:
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0,"x"
	.linkonce discard
	.align 4
.globl _top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	.def	_top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	2;	.type	32;	.endef
_top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0:
	pushl %ebp
	movl %esp,%ebp
	pushl %ebx
	movl 8(%ebp),%ebx
	pushl %ebx
	call _back__t5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%edx
	movl %edx,%eax
	jmp L507
L507:
	movl -4(%ebp),%ebx
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$back__t5deque3ZiZt24__default_alloc_template2b0i0Ui0,"x"
	.linkonce discard
	.align 4
.globl _back__t5deque3ZiZt24__default_alloc_template2b0i0Ui0
	.def	_back__t5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	2;	.type	32;	.endef
_back__t5deque3ZiZt24__default_alloc_template2b0i0Ui0:
	pushl %ebp
	movl %esp,%ebp
	subl $16,%esp
	pushl %ebx
	movl 8(%ebp),%ebx
	leal 16(%ebx),%eax
	pushl %eax
	leal -16(%ebp),%eax
	pushl %eax
	call ___t16__deque_iterator4ZiZRiZPiUi0RCt16__deque_iterator4ZiZRiZPiUi0
	addl $8,%esp
	movl %eax,%eax
	leal -16(%ebp),%eax
	pushl %eax
	call ___mm__t16__deque_iterator4ZiZRiZPiUi0
	addl $4,%esp
	leal -16(%ebp),%eax
	pushl %eax
	call ___ml__Ct16__deque_iterator4ZiZRiZPiUi0
	addl $4,%esp
	movl %eax,%edx
	movl %edx,%eax
	jmp L508
L508:
	movl -20(%ebp),%ebx
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$__ml__Ct16__deque_iterator4ZiZRiZPiUi0,"x"
	.linkonce discard
	.align 4
.globl ___ml__Ct16__deque_iterator4ZiZRiZPiUi0
	.def	___ml__Ct16__deque_iterator4ZiZRiZPiUi0;	.scl	2;	.type	32;	.endef
___ml__Ct16__deque_iterator4ZiZRiZPiUi0:
	pushl %ebp
	movl %esp,%ebp
	movl 8(%ebp),%edx
	movl (%edx),%ecx
	movl %ecx,%eax
	jmp L512
L512:
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$__mm__t16__deque_iterator4ZiZRiZPiUi0,"x"
	.linkonce discard
	.align 4
.globl ___mm__t16__deque_iterator4ZiZRiZPiUi0
	.def	___mm__t16__deque_iterator4ZiZRiZPiUi0;	.scl	2;	.type	32;	.endef
___mm__t16__deque_iterator4ZiZRiZPiUi0:
	pushl %ebp
	movl %esp,%ebp
	pushl %ebx
	movl 8(%ebp),%ebx
	movl (%ebx),%eax
	cmpl 4(%ebx),%eax
	jne L510
	movl 12(%ebx),%eax
	addl $-4,%eax
	pushl %eax
	pushl %ebx
	call _set_node__t16__deque_iterator4ZiZRiZPiUi0PPi
	addl $8,%esp
	movl 8(%ebx),%eax
	movl %eax,(%ebx)
L510:
	addl $-4,(%ebx)
	movl %ebx,%eax
	jmp L509
L509:
	movl -4(%ebp),%ebx
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$set_node__t16__deque_iterator4ZiZRiZPiUi0PPi,"x"
	.linkonce discard
	.align 4
.globl _set_node__t16__deque_iterator4ZiZRiZPiUi0PPi
	.def	_set_node__t16__deque_iterator4ZiZRiZPiUi0PPi;	.scl	2;	.type	32;	.endef
_set_node__t16__deque_iterator4ZiZRiZPiUi0PPi:
	pushl %ebp
	movl %esp,%ebp
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%esi
	movl 12(%ebp),%ebx
	movl %ebx,12(%esi)
	movl (%ebx),%eax
	movl %eax,4(%esi)
	call _buffer_size__t16__deque_iterator4ZiZRiZPiUi0
	movl %eax,%eax
	leal 0(,%eax,4),%edx
	movl 4(%esi),%ecx
	addl %edx,%ecx
	movl %ecx,8(%esi)
L511:
	leal -8(%ebp),%esp
	popl %ebx
	popl %esi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$size__Ct5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0,"x"
	.linkonce discard
	.align 4
.globl _size__Ct5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	.def	_size__Ct5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	2;	.type	32;	.endef
_size__Ct5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0:
	pushl %ebp
	movl %esp,%ebp
	pushl %ebx
	movl 8(%ebp),%ebx
	pushl %ebx
	call _size__Ct5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl %eax,%edx
	movl %edx,%eax
	jmp L501
L501:
	movl -4(%ebp),%ebx
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$size__Ct5deque3ZiZt24__default_alloc_template2b0i0Ui0,"x"
	.linkonce discard
	.align 4
.globl _size__Ct5deque3ZiZt24__default_alloc_template2b0i0Ui0
	.def	_size__Ct5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	2;	.type	32;	.endef
_size__Ct5deque3ZiZt24__default_alloc_template2b0i0Ui0:
	pushl %ebp
	movl %esp,%ebp
	pushl %ebx
	movl 8(%ebp),%ebx
	pushl %ebx
	leal 16(%ebx),%eax
	pushl %eax
	call ___mi__Ct16__deque_iterator4ZiZRiZPiUi0RCt16__deque_iterator4ZiZRiZPiUi0
	addl $8,%esp
	movl %eax,%edx
	movl %edx,%eax
	jmp L502
L502:
	movl -4(%ebp),%ebx
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$__mi__Ct16__deque_iterator4ZiZRiZPiUi0RCt16__deque_iterator4ZiZRiZPiUi0,"x"
	.linkonce discard
	.align 4
.globl ___mi__Ct16__deque_iterator4ZiZRiZPiUi0RCt16__deque_iterator4ZiZRiZPiUi0
	.def	___mi__Ct16__deque_iterator4ZiZRiZPiUi0RCt16__deque_iterator4ZiZRiZPiUi0;	.scl	2;	.type	32;	.endef
___mi__Ct16__deque_iterator4ZiZRiZPiUi0RCt16__deque_iterator4ZiZRiZPiUi0:
	pushl %ebp
	movl %esp,%ebp
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%ebx
	movl 12(%ebp),%esi
	call _buffer_size__t16__deque_iterator4ZiZRiZPiUi0
	movl %eax,%eax
	movl 12(%ebx),%edx
	movl 12(%esi),%ecx
	subl %ecx,%edx
	movl %edx,%ecx
	movl %ecx,%edx
	sarl $2,%edx
	leal -1(%edx),%ecx
	movl %eax,%edx
	imull %ecx,%edx
	movl (%ebx),%eax
	movl 4(%ebx),%ecx
	subl %ecx,%eax
	movl %eax,%ecx
	movl %ecx,%eax
	sarl $2,%eax
	addl %eax,%edx
	movl 8(%esi),%eax
	movl (%esi),%ecx
	subl %ecx,%eax
	movl %eax,%ecx
	movl %ecx,%eax
	sarl $2,%eax
	addl %eax,%edx
	movl %edx,%eax
	jmp L503
L503:
	leal -8(%ebp),%esp
	popl %ebx
	popl %esi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$buffer_size__t16__deque_iterator4ZiZRiZPiUi0,"x"
	.linkonce discard
	.align 4
.globl _buffer_size__t16__deque_iterator4ZiZRiZPiUi0
	.def	_buffer_size__t16__deque_iterator4ZiZRiZPiUi0;	.scl	2;	.type	32;	.endef
_buffer_size__t16__deque_iterator4ZiZRiZPiUi0:
	pushl %ebp
	movl %esp,%ebp
	pushl $4
	pushl $0
	call ___deque_buf_size__FUiUi
	addl $8,%esp
	movl %eax,%edx
	movl %edx,%eax
	jmp L504
	.p2align 4,,7
L504:
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$empty__Ct5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0,"x"
	.linkonce discard
	.align 4
.globl _empty__Ct5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	.def	_empty__Ct5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	2;	.type	32;	.endef
_empty__Ct5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0:
	pushl %ebp
	movl %esp,%ebp
	pushl %ebx
	movl 8(%ebp),%ebx
	pushl %ebx
	call _empty__Ct5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movb %al,%dl
	movb %dl,%al
	jmp L498
L498:
	movl -4(%ebp),%ebx
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$empty__Ct5deque3ZiZt24__default_alloc_template2b0i0Ui0,"x"
	.linkonce discard
	.align 4
.globl _empty__Ct5deque3ZiZt24__default_alloc_template2b0i0Ui0
	.def	_empty__Ct5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	2;	.type	32;	.endef
_empty__Ct5deque3ZiZt24__default_alloc_template2b0i0Ui0:
	pushl %ebp
	movl %esp,%ebp
	pushl %ebx
	movl 8(%ebp),%ebx
	pushl %ebx
	leal 16(%ebx),%eax
	pushl %eax
	call ___eq__Ct16__deque_iterator4ZiZRiZPiUi0RCt16__deque_iterator4ZiZRiZPiUi0
	addl $8,%esp
	movb %al,%dl
	movb %dl,%al
	jmp L499
L499:
	movl -4(%ebp),%ebx
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$__eq__Ct16__deque_iterator4ZiZRiZPiUi0RCt16__deque_iterator4ZiZRiZPiUi0,"x"
	.linkonce discard
	.align 4
.globl ___eq__Ct16__deque_iterator4ZiZRiZPiUi0RCt16__deque_iterator4ZiZRiZPiUi0
	.def	___eq__Ct16__deque_iterator4ZiZRiZPiUi0RCt16__deque_iterator4ZiZRiZPiUi0;	.scl	2;	.type	32;	.endef
___eq__Ct16__deque_iterator4ZiZRiZPiUi0RCt16__deque_iterator4ZiZRiZPiUi0:
	pushl %ebp
	movl %esp,%ebp
	pushl %ebx
	movl 8(%ebp),%edx
	movl 12(%ebp),%ecx
	movl (%edx),%eax
	cmpl (%ecx),%eax
	sete %bl
	movb %bl,%al
	jmp L500
L500:
	movl -4(%ebp),%ebx
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$push__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi,"x"
	.linkonce discard
	.align 4
.globl _push__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi
	.def	_push__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi;	.scl	2;	.type	32;	.endef
_push__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi:
	pushl %ebp
	movl %esp,%ebp
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%ebx
	movl 12(%ebp),%esi
	pushl %esi
	pushl %ebx
	call _push_back__t5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi
	addl $8,%esp
L490:
	leal -8(%ebp),%esp
	popl %ebx
	popl %esi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$push_back__t5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi,"x"
	.linkonce discard
	.align 4
.globl _push_back__t5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi
	.def	_push_back__t5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi;	.scl	2;	.type	32;	.endef
_push_back__t5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi:
	pushl %ebp
	movl %esp,%ebp
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%esi
	movl 12(%ebp),%ebx
	movl 24(%esi),%eax
	addl $-4,%eax
	cmpl %eax,16(%esi)
	je L492
	pushl %ebx
	movl 16(%esi),%eax
	pushl %eax
	call _construct__H2ZiZi_PX01RCX11_v
	addl $8,%esp
	addl $4,16(%esi)
	jmp L495
	.p2align 4,,7
L492:
	pushl %ebx
	pushl %esi
	call _push_back_aux__t5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi
	addl $8,%esp
L495:
L491:
	leal -8(%ebp),%esp
	popl %ebx
	popl %esi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$construct__H2ZiZi_PX01RCX11_v,"x"
	.linkonce discard
	.align 4
.globl _construct__H2ZiZi_PX01RCX11_v
	.def	_construct__H2ZiZi_PX01RCX11_v;	.scl	2;	.type	32;	.endef
_construct__H2ZiZi_PX01RCX11_v:
	pushl %ebp
	movl %esp,%ebp
	pushl %edi
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%ebx
	movl 12(%ebp),%esi
	pushl %ebx
	pushl $4
	call ___nw__FUiPv
	addl $8,%esp
	movl %eax,%eax
	movl %eax,%edx
	movl %edx,%eax
	testl %edx,%edx
	je L494
	movl %edx,%edi
	movl (%esi),%edx
	movl %edx,(%edi)
	movl %edi,%eax
L494:
L493:
	leal -12(%ebp),%esp
	popl %ebx
	popl %esi
	popl %edi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$__t16__deque_iterator4ZiZRiZPiUi0RCt16__deque_iterator4ZiZRiZPiUi0,"x"
	.linkonce discard
	.align 4
.globl ___t16__deque_iterator4ZiZRiZPiUi0RCt16__deque_iterator4ZiZRiZPiUi0
	.def	___t16__deque_iterator4ZiZRiZPiUi0RCt16__deque_iterator4ZiZRiZPiUi0;	.scl	2;	.type	32;	.endef
___t16__deque_iterator4ZiZRiZPiUi0RCt16__deque_iterator4ZiZRiZPiUi0:
	pushl %ebp
	movl %esp,%ebp
	movl 8(%ebp),%ecx
	movl 12(%ebp),%edx
	movl (%edx),%eax
	movl %eax,(%ecx)
	movl 4(%edx),%eax
	movl %eax,4(%ecx)
	movl 8(%edx),%eax
	movl %eax,8(%ecx)
	movl 12(%edx),%eax
	movl %eax,12(%ecx)
L468:
	movl %ecx,%eax
	jmp L467
L467:
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0,"x"
	.linkonce discard
	.align 4
.globl ___t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0
	.def	___t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	2;	.type	32;	.endef
___t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0:
	pushl %ebp
	movl %esp,%ebp
	subl $64,%esp
	pushl %edi
	pushl %esi
	pushl %ebx
	call ___get_eh_context
	movl %eax,%eax
	movl %eax,%edx
	movl %edx,%eax
	movl %eax,-60(%ebp)
	movl 8(%ebp),%ebx
	pushl %ebx
	call ___t5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
	movl -60(%ebp),%eax
	addl $4,%eax
	movl (%eax),%edx
	movl %edx,-24(%ebp)
	movl $0,-20(%ebp)
	leal -16(%ebp),%edx
	movl %ebp,(%edx)
	movl $L479,4(%edx)
	movl %esp,8(%edx)
	xorl %edx,%edx
	jmp L478
	.p2align 4,,7
L479:
	movl %ebp,%ebp
	movl $1,%edx
	jmp L477
	.p2align 4,,7
L478:
	leal -24(%ebp),%ebx
	movl %ebx,(%eax)
	movl -60(%ebp),%eax
	addl $4,%eax
	movl (%eax),%edx
	movl (%edx),%ecx
	movl %ecx,(%eax)
L480:
	movl 8(%ebp),%eax
	jmp L461
	jmp L485
	.p2align 4,,7
L477:
	movl -60(%ebp),%eax
	addl $4,%eax
	movl (%eax),%edx
	movl %edx,-56(%ebp)
	movl $0,-52(%ebp)
	leal -48(%ebp),%edx
	movl %ebp,(%edx)
	movl $L484,4(%edx)
	movl %esp,8(%edx)
	xorl %edx,%edx
	jmp L483
	.p2align 4,,7
L484:
	movl %ebp,%ebp
	movl $1,%edx
	jmp L482
	.p2align 4,,7
L483:
	leal -56(%ebp),%ebx
	movl %ebx,(%eax)
	pushl $0
	movl 8(%ebp),%ebx
	pushl %ebx
	call __$_t5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $8,%esp
	movl -60(%ebp),%eax
	addl $4,%eax
	movl (%eax),%edx
	movl (%edx),%ecx
	movl %ecx,(%eax)
	call ___sjthrow
	.p2align 4,,7
L482:
	call ___terminate
	.p2align 4,,7
L485:
L461:
	leal -76(%ebp),%esp
	popl %ebx
	popl %esi
	popl %edi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$__deque_buf_size__FUiUi,"x"
	.linkonce discard
	.align 4
.globl ___deque_buf_size__FUiUi
	.def	___deque_buf_size__FUiUi;	.scl	2;	.type	32;	.endef
___deque_buf_size__FUiUi:
	pushl %ebp
	movl %esp,%ebp
	subl $16,%esp
	pushl %edi
	pushl %esi
	pushl %ebx
	movl 12(%ebp),%ecx
	movl 8(%ebp),%eax
	movl %eax,-4(%ebp)
	cmpl $0,8(%ebp)
	jne L376
	cmpl $511,%ecx
	ja L377
	movl $512,%edi
	movl %edi,%eax
	xorl %edx,%edx
	divl %ecx
	movl %eax,%esi
	movl %esi,-4(%ebp)
	jmp L376
	.p2align 4,,7
L377:
	movl $1,-4(%ebp)
L378:
L376:
	movl -4(%ebp),%eax
	jmp L375
L375:
	leal -28(%ebp),%esp
	popl %ebx
	popl %esi
	popl %edi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$_$_t5deque3ZiZt24__default_alloc_template2b0i0Ui0,"x"
	.linkonce discard
	.align 4
.globl __$_t5deque3ZiZt24__default_alloc_template2b0i0Ui0
	.def	__$_t5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	2;	.type	32;	.endef
__$_t5deque3ZiZt24__default_alloc_template2b0i0Ui0:
	pushl %ebp
	movl %esp,%ebp
	subl $32,%esp
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%esi
	movl 12(%ebp),%ebx
	pushl %esi
	leal -16(%ebp),%eax
	pushl %eax
	call ___t16__deque_iterator4ZiZRiZPiUi0RCt16__deque_iterator4ZiZRiZPiUi0
	addl $8,%esp
	movl %eax,%eax
	leal 16(%esi),%edx
	pushl %edx
	leal -32(%ebp),%eax
	pushl %eax
	call ___t16__deque_iterator4ZiZRiZPiUi0RCt16__deque_iterator4ZiZRiZPiUi0
	addl $8,%esp
	movl %eax,%eax
	leal -32(%ebp),%edx
	pushl %edx
	leal -16(%ebp),%eax
	pushl %eax
	call _destroy__H1Zt16__deque_iterator4ZiZRiZPiUi0_X01T0_v
	addl $8,%esp
	pushl %esi
	call _destroy_map_and_nodes__t5deque3ZiZt24__default_alloc_template2b0i0Ui0
	addl $4,%esp
L473:
	movl %ebx,%eax
	andl $1,%eax
	testl %eax,%eax
	je L475
	pushl %esi
	call ___builtin_delete
	addl $4,%esp
	jmp L475
	.p2align 4,,7
L474:
L475:
L466:
	leal -40(%ebp),%esp
	popl %ebx
	popl %esi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$destroy__H1Zt16__deque_iterator4ZiZRiZPiUi0_X01T0_v,"x"
	.linkonce discard
	.align 4
.globl _destroy__H1Zt16__deque_iterator4ZiZRiZPiUi0_X01T0_v
	.def	_destroy__H1Zt16__deque_iterator4ZiZRiZPiUi0_X01T0_v;	.scl	2;	.type	32;	.endef
_destroy__H1Zt16__deque_iterator4ZiZRiZPiUi0_X01T0_v:
	pushl %ebp
	movl %esp,%ebp
	subl $32,%esp
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%esi
	movl 12(%ebp),%ebx
	pushl %esi
	leal -16(%ebp),%eax
	pushl %eax
	call ___t16__deque_iterator4ZiZRiZPiUi0RCt16__deque_iterator4ZiZRiZPiUi0
	addl $8,%esp
	movl %eax,%eax
	pushl %ebx
	leal -32(%ebp),%eax
	pushl %eax
	call ___t16__deque_iterator4ZiZRiZPiUi0RCt16__deque_iterator4ZiZRiZPiUi0
	addl $8,%esp
	movl %eax,%eax
	pushl %esi
	call _value_type__H1Zt16__deque_iterator4ZiZRiZPiUi0_RCX01_PQ2t15iterator_traits1ZX0110value_type
	addl $4,%esp
	movl %eax,%eax
	pushl %eax
	leal -32(%ebp),%eax
	pushl %eax
	leal -16(%ebp),%eax
	pushl %eax
	call ___destroy__H2Zt16__deque_iterator4ZiZRiZPiUi0Zi_X01T0PX11_v
	addl $12,%esp
L469:
	leal -40(%ebp),%esp
	popl %ebx
	popl %esi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$__destroy__H2Zt16__deque_iterator4ZiZRiZPiUi0Zi_X01T0PX11_v,"x"
	.linkonce discard
	.align 4
.globl ___destroy__H2Zt16__deque_iterator4ZiZRiZPiUi0Zi_X01T0PX11_v
	.def	___destroy__H2Zt16__deque_iterator4ZiZRiZPiUi0Zi_X01T0PX11_v;	.scl	2;	.type	32;	.endef
___destroy__H2Zt16__deque_iterator4ZiZRiZPiUi0Zi_X01T0PX11_v:
	pushl %ebp
	movl %esp,%ebp
	subl $32,%esp
	pushl %edi
	pushl %esi
	pushl %ebx
	movl 8(%ebp),%esi
	movl 12(%ebp),%edi
	movl 16(%ebp),%ebx
	pushl %esi
	leal -16(%ebp),%eax
	pushl %eax
	call ___t16__deque_iterator4ZiZRiZPiUi0RCt16__deque_iterator4ZiZRiZPiUi0
	addl $8,%esp
	movl %eax,%eax
	pushl %edi
	leal -32(%ebp),%eax
	pushl %eax
	call ___t16__deque_iterator4ZiZRiZPiUi0RCt16__deque_iterator4ZiZRiZPiUi0
	addl $8,%esp
	movl %eax,%eax
	movb $0,%dl
	addl $-2,%esp
	pushw $0
	leal -32(%ebp),%eax
	pushl %eax
	leal -16(%ebp),%eax
	pushl %eax
	call ___destroy_aux__H1Zt16__deque_iterator4ZiZRiZPiUi0_X01T0G11__true_type_v
	addl $12,%esp
L471:
	leal -44(%ebp),%esp
	popl %ebx
	popl %esi
	popl %edi
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$__destroy_aux__H1Zt16__deque_iterator4ZiZRiZPiUi0_X01T0G11__true_type_v,"x"
	.linkonce discard
	.align 4
.globl ___destroy_aux__H1Zt16__deque_iterator4ZiZRiZPiUi0_X01T0G11__true_type_v
	.def	___destroy_aux__H1Zt16__deque_iterator4ZiZRiZPiUi0_X01T0G11__true_type_v;	.scl	2;	.type	32;	.endef
___destroy_aux__H1Zt16__deque_iterator4ZiZRiZPiUi0_X01T0G11__true_type_v:
	pushl %ebp
	movl %esp,%ebp
	movl 8(%ebp),%eax
	movl 12(%ebp),%edx
	movb 16(%ebp),%cl
L472:
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$value_type__H1Zt16__deque_iterator4ZiZRiZPiUi0_RCX01_PQ2t15iterator_traits1ZX0110value_type,"x"
	.linkonce discard
	.align 4
.globl _value_type__H1Zt16__deque_iterator4ZiZRiZPiUi0_RCX01_PQ2t15iterator_traits1ZX0110value_type
	.def	_value_type__H1Zt16__deque_iterator4ZiZRiZPiUi0_RCX01_PQ2t15iterator_traits1ZX0110value_type;	.scl	2;	.type	32;	.endef
_value_type__H1Zt16__deque_iterator4ZiZRiZPiUi0_RCX01_PQ2t15iterator_traits1ZX0110value_type:
	pushl %ebp
	movl %esp,%ebp
	movl 8(%ebp),%edx
	xorl %eax,%eax
	jmp L470
L470:
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$__t5deque3ZiZt24__default_alloc_template2b0i0Ui0,"x"
	.linkonce discard
	.align 4
.globl ___t5deque3ZiZt24__default_alloc_template2b0i0Ui0
	.def	___t5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	2;	.type	32;	.endef
___t5deque3ZiZt24__default_alloc_template2b0i0Ui0:
	pushl %ebp
	movl %esp,%ebp
	pushl %ebx
	movl 8(%ebp),%ebx
	pushl %ebx
	call ___t16__deque_iterator4ZiZRiZPiUi0
	addl $4,%esp
	leal 16(%ebx),%eax
	pushl %eax
	call ___t16__deque_iterator4ZiZRiZPiUi0
	addl $4,%esp
	movl $0,32(%ebx)
	movl $0,36(%ebx)
	pushl $0
	pushl %ebx
	call _create_map_and_nodes__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Ui
	addl $8,%esp
L465:
	movl %ebx,%eax
	jmp L462
L462:
	movl -4(%ebp),%ebx
	movl %ebp,%esp
	popl %ebp
	ret
.section	.text$__t16__deque_iterator4ZiZRiZPiUi0,"x"
	.linkonce discard
	.align 4
.globl ___t16__deque_iterator4ZiZRiZPiUi0
	.def	___t16__deque_iterator4ZiZRiZPiUi0;	.scl	2;	.type	32;	.endef
___t16__deque_iterator4ZiZRiZPiUi0:
	pushl %ebp
	movl %esp,%ebp
	movl 8(%ebp),%edx
	movl $0,(%edx)
	movl $0,4(%edx)
	movl $0,8(%edx)
	movl $0,12(%edx)
L464:
	movl %edx,%eax
	jmp L463
L463:
	movl %ebp,%esp
	popl %ebp
	ret
.text
	.align 4
___WORD_BIT:
	.long 32
.globl _vars
.data
	.align 32
_vars:
	.space 1024
	.def	___t6vector2ZiZt24__default_alloc_template2b0i0;	.scl	3;	.type	32;	.endef
	.def	__$_t6vector2ZiZt24__default_alloc_template2b0i0;	.scl	3;	.type	32;	.endef
	.def	_deallocate__t6vector2ZiZt24__default_alloc_template2b0i0;	.scl	3;	.type	32;	.endef
	.def	___destroy_aux__H1ZPi_X01T0G11__true_type_v;	.scl	3;	.type	32;	.endef
	.def	___destroy__H2ZPiZi_X01T0PX11_v;	.scl	3;	.type	32;	.endef
	.def	_destroy__H1ZPi_X01T0_v;	.scl	3;	.type	32;	.endef
	.def	___copy_t__H1Zi_PCX01T0PX01G11__true_type_PX01;	.scl	3;	.type	32;	.endef
	.def	___cl__t15__copy_dispatch2ZPiZPiPiN21;	.scl	3;	.type	32;	.endef
	.def	_copy__H2ZPiZPi_X01T0X11_X11;	.scl	3;	.type	32;	.endef
	.def	___uninitialized_copy_aux__H2ZPiZPi_X01T0X11G11__true_type_X11;	.scl	3;	.type	32;	.endef
	.def	___uninitialized_copy__H3ZPiZPiZi_X01T0X11PX21_X11;	.scl	3;	.type	32;	.endef
	.def	_value_type__H1ZPi_RCX01_PQ2t15iterator_traits1ZX0110value_type;	.scl	3;	.type	32;	.endef
	.def	_uninitialized_copy__H2ZPiZPi_X01T0X11_X11;	.scl	3;	.type	32;	.endef
	.def	___copy_backward_t__H1Zi_PCX01T0PX01G11__true_type_PX01;	.scl	3;	.type	32;	.endef
	.def	___cl__t24__copy_backward_dispatch2ZPiZPiPiN21;	.scl	3;	.type	32;	.endef
	.def	_copy_backward__H2ZPiZPi_X01T0X11_X11;	.scl	3;	.type	32;	.endef
	.def	___copy_backward_t__H1ZPi_PCX01T0PX01G11__true_type_PX01;	.scl	3;	.type	32;	.endef
	.def	___cl__t24__copy_backward_dispatch2ZPPiZPPiPPiN21;	.scl	3;	.type	32;	.endef
	.def	_copy_backward__H2ZPPiZPPi_X01T0X11_X11;	.scl	3;	.type	32;	.endef
	.def	___copy_t__H1ZPi_PCX01T0PX01G11__true_type_PX01;	.scl	3;	.type	32;	.endef
	.def	___cl__t15__copy_dispatch2ZPPiZPPiPPiN21;	.scl	3;	.type	32;	.endef
	.def	_copy__H2ZPPiZPPi_X01T0X11_X11;	.scl	3;	.type	32;	.endef
	.def	_reallocate_map__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Uib;	.scl	3;	.type	32;	.endef
	.def	_reserve_map_at_back__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Ui;	.scl	3;	.type	32;	.endef
	.def	___uncatch_exception;	.scl	3;	.type	32;	.endef
	.def	_deallocate__t12simple_alloc2ZPiZt24__default_alloc_template2b0i0PPiUi;	.scl	3;	.type	32;	.endef
	.def	_free;	.scl	3;	.type	32;	.endef
	.def	_deallocate__t23__malloc_alloc_template1i0PvUi;	.scl	3;	.type	32;	.endef
	.def	_deallocate__t24__default_alloc_template2b0i0PvUi;	.scl	3;	.type	32;	.endef
	.def	_deallocate__t12simple_alloc2ZiZt24__default_alloc_template2b0i0PiUi;	.scl	3;	.type	32;	.endef
	.def	_deallocate_node__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Pi;	.scl	3;	.type	32;	.endef
	.def	_allocate__t12simple_alloc2ZiZt24__default_alloc_template2b0i0Ui;	.scl	3;	.type	32;	.endef
	.def	_allocate_node__t5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	3;	.type	32;	.endef
	.def	_chunk_alloc__t24__default_alloc_template2b0i0UiRi;	.scl	3;	.type	32;	.endef
	.def	_refill__t24__default_alloc_template2b0i0Ui;	.scl	3;	.type	32;	.endef
	.def	_ROUND_UP__t24__default_alloc_template2b0i0Ui;	.scl	3;	.type	32;	.endef
	.def	_FREELIST_INDEX__t24__default_alloc_template2b0i0Ui;	.scl	3;	.type	32;	.endef
	.def	_exit;	.scl	3;	.type	32;	.endef
	.def	_oom_malloc__t23__malloc_alloc_template1i0Ui;	.scl	3;	.type	32;	.endef
	.def	_malloc;	.scl	3;	.type	32;	.endef
	.def	_allocate__t23__malloc_alloc_template1i0Ui;	.scl	3;	.type	32;	.endef
	.def	_allocate__t24__default_alloc_template2b0i0Ui;	.scl	3;	.type	32;	.endef
	.def	_allocate__t12simple_alloc2ZPiZt24__default_alloc_template2b0i0Ui;	.scl	3;	.type	32;	.endef
	.def	_max__H1ZUi_RCX01T0_RCX01;	.scl	3;	.type	32;	.endef
	.def	_initial_map_size__t5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	3;	.type	32;	.endef
	.def	_buffer_size__t5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	3;	.type	32;	.endef
	.def	___get_eh_context;	.scl	3;	.type	32;	.endef
	.def	___ls__7ostreamPFR7ostream_R7ostream;	.scl	3;	.type	32;	.endef
	.def	_endl__FR7ostream;	.scl	2;	.type	32;	.endef
	.def	_begin__t6vector2ZiZt24__default_alloc_template2b0i0;	.scl	3;	.type	32;	.endef
	.def	___vc__t6vector2ZiZt24__default_alloc_template2b0i0Ui;	.scl	3;	.type	32;	.endef
	.def	_insert_aux__t6vector2ZiZt24__default_alloc_template2b0i0PiRCi;	.scl	3;	.type	32;	.endef
	.def	_end__t6vector2ZiZt24__default_alloc_template2b0i0;	.scl	3;	.type	32;	.endef
	.def	_push_back__t6vector2ZiZt24__default_alloc_template2b0i0RCi;	.scl	3;	.type	32;	.endef
	.def	_end__Ct6vector2ZiZt24__default_alloc_template2b0i0;	.scl	3;	.type	32;	.endef
	.def	_begin__Ct6vector2ZiZt24__default_alloc_template2b0i0;	.scl	3;	.type	32;	.endef
	.def	_size__Ct6vector2ZiZt24__default_alloc_template2b0i0;	.scl	3;	.type	32;	.endef
	.def	_pop_back_aux__t5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	3;	.type	32;	.endef
	.def	_destroy__H1Zi_PX01_v;	.scl	3;	.type	32;	.endef
	.def	_pop_back__t5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	3;	.type	32;	.endef
	.def	_pop__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	3;	.type	32;	.endef
	.def	___ml__Ct16__deque_iterator4ZiZRiZPiUi0;	.scl	3;	.type	32;	.endef
	.def	_set_node__t16__deque_iterator4ZiZRiZPiUi0PPi;	.scl	3;	.type	32;	.endef
	.def	___mm__t16__deque_iterator4ZiZRiZPiUi0;	.scl	3;	.type	32;	.endef
	.def	_back__t5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	3;	.type	32;	.endef
	.def	_top__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	3;	.type	32;	.endef
	.def	___deque_buf_size__FUiUi;	.scl	2;	.type	32;	.endef
	.def	_buffer_size__t16__deque_iterator4ZiZRiZPiUi0;	.scl	3;	.type	32;	.endef
	.def	___mi__Ct16__deque_iterator4ZiZRiZPiUi0RCt16__deque_iterator4ZiZRiZPiUi0;	.scl	3;	.type	32;	.endef
	.def	_size__Ct5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	3;	.type	32;	.endef
	.def	_size__Ct5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	3;	.type	32;	.endef
	.def	___eq__Ct16__deque_iterator4ZiZRiZPiUi0RCt16__deque_iterator4ZiZRiZPiUi0;	.scl	3;	.type	32;	.endef
	.def	_empty__Ct5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	3;	.type	32;	.endef
	.def	_empty__Ct5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	3;	.type	32;	.endef
	.def	_push_back_aux__t5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi;	.scl	3;	.type	32;	.endef
	.def	___nw__FUiPv;	.scl	2;	.type	32;	.endef
	.def	_construct__H2ZiZi_PX01RCX11_v;	.scl	3;	.type	32;	.endef
	.def	_push_back__t5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi;	.scl	3;	.type	32;	.endef
	.def	_push__t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0RCi;	.scl	3;	.type	32;	.endef
	.def	_destroy_map_and_nodes__t5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	3;	.type	32;	.endef
	.def	___destroy_aux__H1Zt16__deque_iterator4ZiZRiZPiUi0_X01T0G11__true_type_v;	.scl	3;	.type	32;	.endef
	.def	___destroy__H2Zt16__deque_iterator4ZiZRiZPiUi0Zi_X01T0PX11_v;	.scl	3;	.type	32;	.endef
	.def	_value_type__H1Zt16__deque_iterator4ZiZRiZPiUi0_RCX01_PQ2t15iterator_traits1ZX0110value_type;	.scl	3;	.type	32;	.endef
	.def	_destroy__H1Zt16__deque_iterator4ZiZRiZPiUi0_X01T0_v;	.scl	3;	.type	32;	.endef
	.def	___t16__deque_iterator4ZiZRiZPiUi0RCt16__deque_iterator4ZiZRiZPiUi0;	.scl	3;	.type	32;	.endef
	.def	__$_t5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	3;	.type	32;	.endef
	.def	_create_map_and_nodes__t5deque3ZiZt24__default_alloc_template2b0i0Ui0Ui;	.scl	3;	.type	32;	.endef
	.def	___t16__deque_iterator4ZiZRiZPiUi0;	.scl	3;	.type	32;	.endef
	.def	___t5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	3;	.type	32;	.endef
	.def	___t5stack2ZiZt5deque3ZiZt24__default_alloc_template2b0i0Ui0;	.scl	3;	.type	32;	.endef
	.def	___builtin_new;	.scl	3;	.type	32;	.endef
	.def	_lexicographical_compare_3way__H2ZPCScZPCSc_X01T0X11T2_i;	.scl	3;	.type	32;	.endef
	.def	_lexicographical_compare__H2ZPCScZPCSc_X01T0X11T2_b;	.scl	3;	.type	32;	.endef
	.def	_memmove;	.scl	3;	.type	32;	.endef
	.def	___ls__7ostreamUi;	.scl	3;	.type	32;	.endef
	.def	___ls__7ostreami;	.scl	3;	.type	32;	.endef
	.def	___ls__7ostreamPCc;	.scl	3;	.type	32;	.endef
	.def	___ls__7ostreamc;	.scl	3;	.type	32;	.endef
	.def	___cp_pop_exception;	.scl	3;	.type	32;	.endef
	.def	___cp_eh_info;	.scl	3;	.type	32;	.endef
	.def	___builtin_delete;	.scl	3;	.type	32;	.endef

#include <stdio.h>

struct {
  int l = 1;
  int m = 2;
} q;
struct {
  int a = 3;
  int l;
  int m = 4;
} r;
int main()
{
  printf("%i\n", q.l);
  printf("%i\n", q.m);
  printf("%i\n", r.a);
  printf("%i\n", r.m);
  return 0;
}
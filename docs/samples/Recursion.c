#include <stdio.h>

int gcd(int a, int b)
{
  if (a > b) return gcd(b, a);
  else if (a == 0) return b;
  else return gcd(b % a, a);
}
int main()
{
  // 184382 = 578 * 319, 7514 = 578 * 13
  printf("%i\n", gcd(184382, 7514));
  return 0;
}
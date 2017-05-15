#include <stdio.h>

int main()
{
  int height = 5;
  for (int i = 0; i < height; i++) {
    int padding = height - i;
    for (int j = 0; j < padding; j++)
      printf(" ");
    int rowLength = 2 * i;
    for (int j = 0; j <= rowLength; j++)
      printf("*");
    printf("\n");
  }
  return 0;
}
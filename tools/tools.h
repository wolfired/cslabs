#ifndef TOOLS_H
#define TOOLS_H

#include <ctype.h>
#include <stdlib.h>
#include <string.h>

__declspec(dllexport) int tools_max(int x, int y);
__declspec(dllexport) char *tools_toupper(const char *src);

#endif

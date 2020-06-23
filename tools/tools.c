#include "tools.h"

int tools_max(int x, int y)
{
    return x > y ? x : y;
}

char *tools_toupper(const char *src)
{
    size_t size = (strlen(src) + 1) * sizeof(char);

    char *dst = (char *)malloc(size);

    if (NULL == dst)
    {
        return NULL;
    }

    strcpy(dst, src);

    for (size_t i = 0; i < size; ++i)
    {
        dst[i] = toupper(dst[i]);
    }

    return dst;
}

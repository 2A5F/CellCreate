#include "String.h"

#include <mimalloc.h>

using namespace cc;

String16::String16(const size_t len) : m_len(len)
{
}

void String16::Free(void* self) noexcept
{
    delete[] static_cast<char*>(self);
}

FrStr16 String16::AsStr() const noexcept
{
    return FrStr16{data(), size()};
}

Rc<String16> String16::Create(const FrStr16 str)
{
    auto ptr = new char[sizeof(String16) + str.size() * sizeof(Char16) + sizeof(Char16)];
    Rc r = new(ptr)String16(str.size());
    memcpy(ptr + sizeof(String16), str.data(), str.size() * sizeof(Char16));
    *reinterpret_cast<Char16*>(ptr + sizeof(String16) + str.size() * sizeof(Char16)) = 0;
    return r;
}

const Char16* String16::data() const noexcept
{
    return reinterpret_cast<const Char16*>(this + sizeof(String16));
}

size_t String16::size() const noexcept
{
    return m_len;
}

const wchar_t* String16::c_str() const noexcept
{
    return reinterpret_cast<const wchar_t*>(reinterpret_cast<const char*>(this) + sizeof(String16));
}

std::wstring_view String16::AsStdView() const
{
    return {c_str(), size()};
}

std::wstring String16::AsStd() const
{
    return {c_str(), size()};
}

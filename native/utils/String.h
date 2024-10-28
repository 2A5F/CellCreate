#pragma once
#include "../Object.h"
#include "../ffi/FFI.h"

namespace cc
{
    class String16 final : public Object<FString16>
    {
        IMPL_OBJECT();

        size_t m_len;

        explicit String16(size_t len);

        void Free(void* self) noexcept override;

    public:
        FrStr16 AsStr() const noexcept override;

        static Rc<String16> Create(FrStr16 str);

        const Char16* data() const noexcept;
        size_t size() const noexcept;

        const wchar_t* c_str() const noexcept;

        std::wstring_view AsStdView() const;
        std::wstring AsStd() const;
    };
} // cc

/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Mô tả    : Base Controller cho toàn bộ trang Admin MVC
              - Tất cả Controller admin kế thừa từ đây
              - Tự động dùng Cookie scheme, không ảnh hưởng JWT của API React
*/
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Backend.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class BaseAdminController : Controller
    {
        // Không cần gì thêm, chỉ để kế thừa
    }
}
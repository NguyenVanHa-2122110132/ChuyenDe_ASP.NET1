/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Cập nhật blogService - Bẫy lỗi try/catch trực tiếp tại Service để chặn sập
               Interceptor Axios - Phục vụ trang Tin Tức & Xu Hướng Thời Trang
*/
import axiosClient from '../api/axiosClient';

const blogService = {
    // Hàm gọi API lấy danh mục các chủ đề bài viết (Xu hướng, Tips phối đồ, Review...)
    getBlogCategories: async () => {
        try {
            const url = '/Categories'; // Khớp với Route quản lý chuyên mục tin tức ở Backend
            const res = await axiosClient.get(url);
            return res;
        } catch (error) {
            // Trả về mảng rỗng ngay lập tức để chặn lỗi parse "undefined" JSON
            console.warn('Backend chưa cấu hình Route /Categories hoặc chưa có dữ liệu mẫu:', error.message);
            return [];
        }
    },

    // Hàm lấy danh sách toàn bộ bài viết xu hướng thời trang từ Backend
    getAllPosts: async () => {
        try {
            const url = '/Posts'; // Khớp với Route quản lý bài viết ở Backend
            const res = await axiosClient.get(url);
            return res;
        } catch (error) {
            console.error('Lỗi gọi API /Posts:', error.message);
            return [];
        }
    },

    // Hàm lấy chi tiết một bài viết theo ID (dùng cho trang đọc bài viết)
    getPostById: async (id) => {
        try {
            const url = `/Posts/${id}`;
            const res = await axiosClient.get(url);
            return res;
        } catch (error) {
            console.error(`Lỗi gọi API /Posts/${id}:`, error.message);
            return null;
        }
    },
};

export default blogService;

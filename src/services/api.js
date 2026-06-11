/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Cấu hình kết nối Axios tập trung hướng tới Web API của shop thời trang
               Tổ chức theo từng cụm chức năng: Bài viết, Danh mục, Sản phẩm, Xác thực, Đơn hàng
*/
import axios from 'axios';

// Khởi tạo cấu hình mạng cơ bản - đọc từ .env để dễ deploy
const api = axios.create({
    baseURL: process.env.REACT_APP_API_URL || 'https://localhost:7038/api',
    headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
    },
    withCredentials: true,
});

// --- CỤM 1: API BÀI VIẾT & XU HƯỚNG THỜI TRANG (POSTS API) ---
export const postsApi = {
    getAllPosts: () => api.get('/Posts'),
    getPostById: (id) => api.get(`/Posts/${id}`),
};

// --- CỤM 2: API DANH MỤC THỜI TRANG (CATEGORIES PRODUCTS API) ---
//    Bao gồm: Áo, Quần, Váy & Đầm, Giày, Túi xách, Phụ kiện...
export const categoriesApi = {
    getCategoryList: () => api.get('/CategoriesProducts'),
};

// --- CỤM 3: API SẢN PHẨM THỜI TRANG (PRODUCTS API) ---
export const productsApi = {
    // Lấy sản phẩm theo mã danh mục (ví dụ: lọc "Áo", "Quần"...)
    getProductsByCategory: (categoryId) => api.get(`/Products/categoryproduct/${categoryId}`),
    // Xem chi tiết sản phẩm (tên, màu sắc, size, giá, ảnh...)
    getProductById: (id) => api.get(`/Products/${id}`),
    // Lấy toàn bộ sản phẩm (dùng cho trang Shop tổng hợp)
    getAllProducts: () => api.get('/Products'),
};

// --- CỤM 4: API KHÁCH HÀNG & XÁC THỰC (CUSTOMER API) ---
export const authApi = {
    customerLogin: (loginData) => api.post('/Customer/login', loginData),
    customerRegister: (registerData) => api.post('/Customer/register', registerData),
};

// --- CỤM 5: API XỬ LÝ ĐƠN HÀNG MUA HÀNG THỜI TRANG (ORDERS API) ---
export const ordersApi = {
    createOrder: (orderData) => api.post('/Orders', orderData),
    getOrderById: (id) => api.get(`/Orders/${id}`),
    getOrdersByCustomer: (customerId) => api.get(`/Orders/customer/${customerId}`),
};

export default api;

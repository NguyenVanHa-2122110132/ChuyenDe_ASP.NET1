/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Cấu hình HTTP Client (Axios) - Bổ sung bẫy lỗi an toàn cho Interceptor Response
               Đọc baseURL từ biến môi trường để dễ deploy lên server thật
*/
import axios from 'axios';

// Đọc URL từ file .env (REACT_APP_API_URL), nếu không có thì dùng localhost
const axiosClient = axios.create({
    baseURL: process.env.REACT_APP_API_URL || 'https://localhost:7038/api',
    headers: {
        'Content-Type': 'application/json',
    },
    timeout: 10000, // Giới hạn 10 giây, tránh treo giao diện vô tận
});

// --- INTERCEPTOR REQUEST: Tự động gắn token xác thực vào mỗi request ---
axiosClient.interceptors.request.use(
    (config) => {
        // Lấy token từ localStorage (sau này khi làm đăng nhập sẽ lưu vào đây)
        const token = localStorage.getItem('token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

// --- INTERCEPTOR RESPONSE: Trả data thẳng, bẫy lỗi an toàn ---
axiosClient.interceptors.response.use(
    (response) => {
        // Trả về data trực tiếp giúp các component lấy dữ liệu mượt mà, không cần .data
        return response.data || response;
    },
    (error) => {
        // Bọc an toàn kiểm tra lỗi để chặn đứng các tác nhân đọc trộm dữ liệu vỡ JSON
        if (error && error.response) {
            console.error(`API trả về mã lỗi: ${error.response.status}`);
            return Promise.reject(error.response.data || error.message);
        }
        console.error('Lỗi kết nối API hoặc lỗi mạng hệ thống:', error.message);
        return Promise.reject(error);
    }
);

export default axiosClient;

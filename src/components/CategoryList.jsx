/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Component hiển thị danh sách hãng điện thoại (iPhone, Samsung...) bằng cách gọi API từ Backend
*/

import React, { useState, useEffect } from 'react'; // Nhập các Hook quản lý trạng thái và vòng đời component của React
import { categoriesApi } from '../services/api';   // Nhập hàm gọi API danh mục hãng điện thoại từ file api.js tập trung

const CategoryList = () => {
    // 1. Khai báo trạng thái (State) để lưu trữ danh sách các hãng điện thoại sau khi lấy từ SQL Server về
    const [categories, setCategories] = useState([]);

    // 2. Khai báo trạng thái để quản lý hiệu ứng vòng xoay đang tải dữ liệu (Loading)
    const [loading, setLoading] = useState(true);

    // 3. Khai báo trạng thái để lưu trữ và hiển thị thông báo nếu chẳng may đường truyền mạng bị lỗi
    const [error, setError] = useState(null);

    // 4. Định nghĩa hàm bất đồng bộ (async/await) để tiến hành "bắn" request GET xuống Backend lấy dữ liệu
    const fetchCategories = async () => {
        try {
            setLoading(true); // Bật trạng thái đang tải dữ liệu lên trước khi gọi mạng

            // Thực hiện gọi hàm lấy danh sách danh mục hãng từ trục cấu hình api.js
            const response = await categoriesApi.getCategoryList();

            // Khi Backend trả về chuỗi JSON thành công, nạp mảng dữ liệu đó vào biến trạng thái setCategories
            setCategories(response.data);
        } catch (err) {
            // Nếu có lỗi (ví dụ: tắt Backend, sai cổng Port, lỗi CORS...), lưu lỗi lại để hiện lên màn hình
            console.error("Lỗi khi gọi API danh mục hãng điện thoại:", err);
            setError("Không thể tải danh sách hãng điện thoại. Vui lòng kiểm tra kết nối API Backend!");
        } finally {
            setLoading(false); // Dù chạy thành công hay thất bại thì cũng tắt hiệu ứng Loading đi
        }
    };

    // 5. Sử dụng useEffect để ép buộc Component tự động chạy hàm fetchCategories ngay lần đầu tiên hiển thị lên màn hình
    useEffect(() => {
        fetchCategories(); // Kích hoạt lệnh gọi API tự động
    }, []); // Mảng rỗng [] đảm bảo hàm này chỉ chạy duy nhất 1 lần khi load trang

    // Giao diện hiển thị trạng thái 1: Nếu đang tải dữ liệu từ SQL Server thì hiện chữ thông báo
    if (loading) return <div style={{ padding: '20px', textAlign: 'center' }}>Đang tải danh sách hãng điện thoại...</div>;

    // Giao diện hiển thị trạng thái 2: Nếu bị lỗi kết nối thì hiện khung chữ đỏ cảnh báo
    if (error) return <div style={{ padding: '20px', color: 'red', textAlign: 'center' }}>{error}</div>;

    // Giao diện hiển thị trạng thái 3: Khi có dữ liệu thành công, dùng hàm .map() lặp qua mảng JSON để in ra màn hình các nút bấm hãng máy
    return (
        <div style={{ padding: '20px', backgroundColor: '#f8f9fa', borderRadius: '8px', marginBottom: '20px' }}>
            <h3 style={{ borderBottom: '2px solid #007bff', paddingBottom: '10px', color: '#333' }}>
                Danh Mục Hãng Điện Thoại (Mobile Brands)
            </h3>

            {/* Thanh chứa danh sách các hãng máy */}
            <div style={{ display: 'flex', gap: '15px', marginTop: '15px', flexWrap: 'wrap' }}>
                {categories.length === 0 ? (
                    <p>Hiện tại chưa có hãng điện thoại nào được nạp vào hệ thống.</p>
                ) : (
                    // Vòng lặp .map() bóc tách từng phần tử trong mảng danh mục ra để hiển thị thành nút bấm
                    categories.map((item) => (
                        <button
                            key={item.id} // Cung cấp thuộc tính khóa key duy nhất để React quản lý danh sách hiệu quả
                            style={{
                                padding: '10px 20px',
                                backgroundColor: '#fff',
                                border: '1px solid #ced4da',
                                borderRadius: '20px',
                                cursor: 'pointer',
                                fontWeight: 'bold',
                                boxShadow: '0 2px 4px rgba(0,0,0,0.05)',
                                transition: 'all 0.2s'
                            }}
                            onClick={() => alert(`Hà vừa bấm chọn lọc dòng điện thoại thuộc hãng: ${item.name || 'Chưa rõ tên'}`)}
                        >
                            {/* In tên danh mục (ví dụ: iPhone, Samsung, Xiaomi) lên mặt nút bấm */}
                            {item.name}
                        </button>
                    ))
                )}
            </div>
        </div>
    );
};

// Xuất bản Component này ra để file chính App.js có thể gọi nhúng vào màn hình
export default CategoryList;
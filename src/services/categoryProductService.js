/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Mở rộng - Thêm các hàm gọi API danh mục và sản phẩm thời trang
               từ trục gốc axiosClient - Phục vụ sidebar danh mục bên trái trang chủ
*/
import axiosClient from '../api/axiosClient';

const categoryProductService = {
    // 1. Hàm lấy toàn bộ danh mục thời trang (Áo, Quần, Váy, Giày, Phụ kiện...)
    getAllCategoryProducts: () => {
        const url = '/CategoriesProducts';
        return axiosClient.get(url);
    },

    // 2. Hàm lấy TOÀN BỘ sản phẩm thời trang từ trục /Products
    getAllProducts: () => {
        const url = '/Products';
        return axiosClient.get(url);
    },

    // 3. Hàm lấy sản phẩm lọc theo mã danh mục từ đầu categoryproduct
    //    Ví dụ: categoryId = 1 → lấy tất cả sản phẩm thuộc danh mục "Áo"
    getProductsByCategory: (categoryId) => {
        const url = `/Products/categoryproduct/${categoryId}`;
        return axiosClient.get(url);
    },
};

export default categoryProductService;

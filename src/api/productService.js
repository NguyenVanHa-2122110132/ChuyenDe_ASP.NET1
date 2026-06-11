/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Quản lý các hàm dịch vụ gọi API danh mục và sản phẩm thời trang
               thông qua trục gốc axiosClient - Bổ sung đầy đủ các hàm cần thiết
*/
import axiosClient from './axiosClient';

const productService = {
    // 1. Lấy danh mục theo gender
    getCategories: (gender = null) => {
        if (gender) {
            return axiosClient.get(`/CategoriesProducts?gender=${gender}`);
        }
        return axiosClient.get('/CategoriesProducts');
    },

    // 2. Lấy toàn bộ sản phẩm
    getAllProducts: () => {
        return axiosClient.get('/Products');
    },

    // 3. Lấy sản phẩm theo danh mục (nếu không có categoryId thì lấy tất cả)
    getProductsByCategory: (categoryId = null) => {
        if (!categoryId) {
            return axiosClient.get('/Products');
        }
        return axiosClient.get(`/Products/categoryproduct/${categoryId}`);
    },

    // 4. Lấy chi tiết sản phẩm theo ID
    getProductById: (id) => {
        return axiosClient.get(`/Products/${id}`);
    },
    // 5. Lấy sản phẩm theo gender
    getProductsByGender: (gender) => {
        return axiosClient.get(`/Products/gender/${gender}`);
    },
};

export default productService;
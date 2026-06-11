/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Bước 1 (Sản phẩm) - API Service gọi danh sách sản phẩm thời trang qua axiosClient
               Bổ sung đầy đủ các hàm: getAllProducts, getProductsByCategory, getProductById,
               getCategories
               Lưu ý: File này dùng cho các component trong /components và /pages
*/
import axiosClient from '../api/axiosClient';

const productService = {
    // 1. Lấy toàn bộ danh sách sản phẩm
    getAllProducts: () => {
        return axiosClient.get('/Products');
    },

    // 2. Lấy sản phẩm theo mã danh mục (Áo, Quần, Váy, Giày...)
    getProductsByCategory: (categoryId) => {
        return axiosClient.get(`/Products/categoryproduct/${categoryId}`);
    },

    // 3. Lấy chi tiết một sản phẩm theo ID
    getProductById: (id) => {
        return axiosClient.get(`/Products/${id}`);
    },

    // 4. Lấy danh sách danh mục theo gender - dùng cho sidebar FashionPage
    getCategories: (gender = null) => {
        const url = gender
            ? `/CategoriesProducts?gender=${gender}`
            : '/CategoriesProducts';
        return axiosClient.get(url);
    },
};

export default productService;
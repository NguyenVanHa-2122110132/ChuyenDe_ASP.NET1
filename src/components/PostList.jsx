/*
    Họ và tên: Nguyễn Văn Hà
    MSSV     : 2122110132
    Lớp      : CCQ2211D
    Ngày tạo : 02/06/2026
    Mô tả    : Cập nhật PostList - Danh sách bài viết xu hướng & tips thời trang
               "Xu hướng thời trang & Tips phối đồ"
                thêm placeholder ảnh thời trang
*/
import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import blogService from '../services/blogService';

const PostList = () => {
    const [posts, setPosts] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchPosts = async () => {
            try {
                setLoading(true);
                const data = await blogService.getAllPosts();
                setPosts(data.data || data);
            } catch (error) {
                console.error('Lỗi khi tải danh sách bài viết thời trang:', error);
            } finally {
                setLoading(false);
            }
        };
        fetchPosts();
    }, []);

    if (loading) {
        return <div className="text-center my-4 text-muted">Đang tải xu hướng thời trang...</div>;
    }

    return (
        <div className="mt-5">
            {/* Tiêu đề section "Thời trang" */}
            <h4 className="mb-4 text-uppercase text-danger font-weight-bold border-bottom pb-2">
                <i className="fa-solid fa-fire mr-2"></i> XU HƯỚNG THỜI TRANG & TIPS PHỐI ĐỒ
            </h4>

            {posts.length === 0 ? (
                <p className="text-muted">Chưa có bài viết xu hướng nào.</p>
            ) : (
                <div className="row">
                    {posts.map((post) => (
                        /* Chia lưới 3 cột (lg), 2 cột (md), 1 cột (mobile) */
                        <div className="col-lg-4 col-md-6 col-12 mb-4" key={post.id}>
                            <div className="card h-100 shadow-sm border-0">
                                {/* Ảnh bài viết - fallback sang ảnh thời trang nếu chưa có */}
                                <img
                                    src={
                                        post.imageUrl ||
                                        'https://images.unsplash.com/photo-1490481651871-ab68de25d43d?q=80&w=400'
                                    }
                                    className="card-img-top"
                                    alt={post.title}
                                    style={{ height: '200px', objectFit: 'cover' }}
                                />
                                <div className="card-body">
                                    <h5 className="card-title font-weight-bold">
                                        <Link
                                            to={`/post/${post.id}`}
                                            className="text-dark text-decoration-none"
                                        >
                                            {post.title}
                                        </Link>
                                    </h5>
                                    <p className="card-text text-muted small">
                                        {post.shortDescription || 'Đang cập nhật nội dung tóm tắt...'}
                                    </p>
                                </div>
                                <div className="card-footer bg-white border-0 d-flex justify-content-between align-items-center">
                                    <span className="small text-secondary">
                                        <i className="fa-regular fa-calendar mr-1"></i>
                                        {/* Format ngày theo chuẩn Việt Nam: dd/mm/yyyy */}
                                        {new Date(post.createdDate).toLocaleDateString('vi-VN')}
                                    </span>
                                    <Link
                                        to={`/post/${post.id}`}
                                        className="badge badge-danger px-2 py-1 text-white text-decoration-none"
                                    >
                                        Đọc tiếp
                                    </Link>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
};

export default PostList;

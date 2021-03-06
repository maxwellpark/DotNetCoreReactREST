import api from "../api";
import axios from "axios";

export const getBlogs = () => async (dispatch) => {
  const response = await api.get("/posts").then(({ data }) => {
    dispatch({ type: "SET_BLOGS", payload: data.objList });
  });
};

export const getSingleBlog = (id) => async (dispatch, getState) => {
  dispatch({ type: "SET_BLOG_LOADING", payload: true });
  if (!id) {
    dispatch({ type: "SET_SINGLE_BLOG", payload: null });
    return;
  }
  const blogs = getState().blogs.all;
  const blog = blogs.find((b) => b.id === id);
  if (blog) {
    dispatch({ type: "SET_SINGLE_BLOG", payload: blog });
    return;
  }
  const response = await api.get(`/posts/${id}`);
  dispatch({ type: "SET_SINGLE_BLOG", payload: response.data });
};

export const editBlog = (blog) => async (dispatch) => {
  await api.patch("/posts/" + blog.id, [
    {
      value: blog.title,
      path: "/title",
      op: "replace",
    },
    {
      value: blog.description,
      path: "/content",
      op: "replace",
    },
    {
      path: "/categoryId",
      value: blog.categoryId,
      op: "replace",
    },
  ]);
  dispatch({ type: "UPDATE_BLOG", payload: blog });
};

export const createBlog = (
  categoryId,
  title,
  content,
  applicationUserId
) => async (dispatch) => {
  const response = await api
    .post("/posts", {
      categoryId,
      title,
      content,
      applicationUserId,
    })
    .then(({ data }) => {
      console.log("cbresp:", data);
      dispatch({ type: "SET_NEWLY_CREATED_BLOG", payload: data });
    })
    .catch((error) => console.log(error));
};

export const getSingleBlogComments = (id) => async (dispatch) => {
  const response = await api
    .get(`/posts/${id}/comments`)
    .then(({ data }) => {
      dispatch({ type: "SET_SINGLE_BLOG_COMMENTS", payload: data });
    })
    .catch((error) => console.log(error));
};

export const getSingleBlogLikes = (id) => async (dispatch) => {
  const response = await api
    .get(`/posts/${id}/postlikes`)
    .then(({ data }) => {
      dispatch({ type: "SET_SINGLE_BLOG_LIKES", payload: data });
    })
    .catch((error) => console.log(error));
};

export const getSingleBlogLikeCount = (id) => async (dispatch) => {
  const response = await api
    .get(`/posts/${id}/postlikes`)
    .then(({ data }) => {
      const likeCount = data.filter((d) => d.isLiked).length;
      dispatch({ type: "SET_SINGLE_BLOG_LIKE_COUNT", payload: likeCount });
    })
    .catch((error) => console.log(error));
};

export const likeBlog = (postId, userId) => async (dispatch) => {
  const response = await api
    .post(`posts/${postId}/users/${userId}/postlikes`)
    .then(({ data }) => {
      console.log("likeblogresponse", data);
      dispatch({ type: "SET_SINGLE_BLOG_LIKES", payload: data });
    });
};

export const unlikeBlog = (id) => async (dispatch) => {
  const response = await api.delete(`postlikes/${id}`).then(({ data }) => {
    dispatch({ type: "SET_SINGLE_BLOG_LIKES", payload: data });
  });
};

// export const LikeOrUnlikeSingleBlog = (id, like = true) => async dispatch => {
//   const response = await api;
//   like
//     ? response
//         .post(`posts/${id}/postlikes`)
//         .then(({ data }) =>
//           dispatch({ type: "SET_SINGLE_BLOG_LIKES", payload: data })
//         )
//     : response.delete(`posts/postlikes/${id}`).then(({ data }) => {
//         dispatch({ type: "SET_SINGLE_BLOG_LIKES", payload: data });
//       });
// };

export const getUsers = () => async (dispatch) => {
  const response = await api
    .get("/users")
    .then((data) => dispatch({ type: "SET_USERS", payload: data }));
};

// get all categories, useful for filtering by category
export const getCategories = () => async (dispatch) => {
  const response = await api.get("/categories").then(({ data }) => {
    console.log("categories response: ", data.objList);
    dispatch({ type: "SET_CATEGORIES", payload: data.objList });
  });
};

export const getLikesForComment = (id) => async (dispatch) => {
  const response = await api
    .get(`/comments/${id}/likes`)
    .then((response) =>
      dispatch({ type: "SET_LIKES_FOR_COMMENT", payload: response.data })
    );
};

export const createComment = (
  content,
  postId,
  applicationUserId,
  isAnonymous
) => async (dispatch) => {
  const response = await api
    .post(`/comments/`, {
      content,
      postId,
      applicationUserId,
      isAnonymous,
    })
    .then(({ data }) => {
      dispatch({
        // use local state instead to make new comment immediately visible to commenter?
        type: "CREATE_COMMENT",
        payload: data,
      });
    })
    .catch((error) => console.log(error));
};

export const likeComment = (commentId, userId) => async (dispatch) => {
  const response = await api
    .post(`/api/comments/${commentId}/users/${userId}/likes`)
    .then((data) => {
      dispatch({ type: "SET_LIKES_FOR_COMMENT", payload: data });
    })
    .catch((error) => console.log("likeComment error:", error));
};

export const deleteLike = (id) => async (dispatch) => {
  const response = await api
    .delete(`/likes/${id}`)
    .then((data) => dispatch({ type: "DELETE_LIKE", payload: data }));
};

// export const toggleLiked = (id, target) => async dispatch => {
//   target === "blog"
//     ? dispatch({ type: "TOGGLE_BLOG_LIKE" })
//     : dispatch({ type: "TOGGLE_COMMENT_LIKE" });
// };

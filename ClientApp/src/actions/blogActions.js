import api from "../api";

export const getBlogs = () => async dispatch => {
  const response = await api.get("/posts");
  console.log("getblogsresponse:", response);
  dispatch({ type: "SET_BLOGS", payload: response.data });
};

export const getSingleBlog = id => async (dispatch, getState) => {
  dispatch({ type: "SET_BLOG_LOADING", payload: true });
  if (!id) {
    dispatch({ type: "SET_SINGLE_BLOG", payload: null });
    return;
  }
  const blogs = getState().blogs.all;
  const blog = blogs.find(b => b.id === id);
  if (blog) {
    dispatch({ type: "SET_SINGLE_BLOG", payload: blog });
    return;
  }
  const response = await api.get(`/posts/${id}`);
  console.log(response);
  dispatch({ type: "SET_SINGLE_BLOG", payload: response.data });
};

export const editBlog = blog => async dispatch => {
  console.log(blog);
  await api.patch("/posts/" + blog.id, [
    {
      value: blog.title,
      path: "/title",
      op: "replace"
    },
    {
      value: blog.description,
      path: "/content",
      op: "replace"
    },
    {
      path: "/categoryId",
      value: blog.categoryId,
      op: "replace"
    }
  ]);
  dispatch({ type: "UPDATE_BLOG", payload: blog });
};

export const getSingleBlogComments = id => async dispatch => {
  const responce = api
    .get(`/posts/${id}/comments`)
    .then(data => dispatch({ type: "SET_SINGLE_BLOG_COMMENTS", payload: data }))
    .catch(error => console.log(error));
};

export const getUsers = () => async dispatch => {
  const response = api
    .get("/users")
    .then(data => dispatch({ type: "SET_USERS", payload: data }));
};

// get all categories, useful for filtering by category
export const getCategories = () => async dispatch => {
  const response = await api.get("/categories");
  console.log("categories response: ", response);
  dispatch({ type: "SET_CATEGORIES", payload: response.data });
};

export const getLikesForComment = id => async dispatch => {
  const response = await api
    .get(`/comments/${id}/comments/likes`)
    .then(data => dispatch({ type: "SET_LIKES_FOR_COMMENT", payload: data }));
  console.log("getlikesresponse:", response);
};

export const createComment = comment => async dispatch => {
  const response = await api
    .post(`/comments/${comment.id}`, {
      id: comment.id,
      content: comment.content,
      postId: comment.postId,
      applicationUserId: user.applicationUserId,
      dateCreated: comment.date,
      isAnonymous: comment.IsAnonymous
    })
    .then(data =>
      dispatch({
        // use local state instead to make new comment immediately visible to commenter?
        type: "SET_SINGLE_BLOG_COMMENTS",
        payload: data
      }).catch(error => console.log(error))
    );
};

// export const toggleLiked = (id, target) => async dispatch => {
//   target === "blog"
//     ? dispatch({ type: "TOGGLE_BLOG_LIKE" })
//     : dispatch({ type: "TOGGLE_COMMENT_LIKE" });
// };

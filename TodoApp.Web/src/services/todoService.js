import axios from 'axios';

const apiClient = axios.create({
  baseURL: '/api/TodoItems', 
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json'
  }
});

apiClient.interceptors.response.use(
  response => response, 
  error => {
    console.error('API Error:', error);
    console.error('API Error Response:', error.response);
    console.error('API Error Request:', error.request);

    let message = 'An unexpected error occurred.';
    if (error.response) {
      if (error.response.data && error.response.data.error) {
        message = error.response.data.error; 
      } else if (error.response.statusText) {
        message = `Error ${error.response.status}: ${error.response.statusText}`;
      }
    } else if (error.request) {
      message = 'Could not connect to the server. Please check your network or try again later.';
    } else {
      message = error.message;
    }

    return Promise.reject(new Error(message));
  }
);


export default {
  getAllItems() {
    return apiClient.get('');
  },
  getItemById(id) {
    return apiClient.get(`/${id}`);
  },
  addItem(item) {
    const payload = {
        title: item.title,
        description: item.description || '',
        category: item.category
    };
    return apiClient.post('', payload);
  },
  updateItem(id, item) {
     const payload = {
        title: item.title, 
        description: item.description || '',
        category: item.category
    };
    return apiClient.put(`/${id}`, payload);
  },
  deleteItem(id) {
    return apiClient.delete(`/${id}`);
  },
  registerProgression(id, progression) {
     const payload = {
        date: new Date(progression.date).toISOString(),
        percent: progression.percent
     };
    return apiClient.post(`/${id}/progressions`, payload);
  },
  getCategories() {
    return apiClient.get('/categories');
  },
  getPrintOutput() {
    return apiClient.get('/print');
  }
};
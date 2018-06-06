import axios from 'axios';

export default {
  fetchApplicationInfo(success, failure) {
    axios.get('api/app/info')
      .then(response => success(response.data))
      .catch(error => failure(error.response));
  },
};

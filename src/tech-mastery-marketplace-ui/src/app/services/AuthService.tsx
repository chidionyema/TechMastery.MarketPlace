// components/AuthService.ts
import axios from 'axios';

export async function register(email: string, password: string) {
  const response = await axios.post('/api/account/register', { email, password });
  return response.data;
}

export async function login(email: string, password: string) {
  const response = await axios.post('/api/account/login', { email, password });
  return response.data;
}

export async function requestPasswordReset(email: string) {
  const response = await axios.post('/api/account/requestPasswordReset', { email });
  return response.data;
}

// components/UserService.ts
import axios from 'axios';

export async function updateUserProfile(userId: string, profileData: any) {
  const response = await axios.put(`/api/user/${userId}`, profileData);
  return response.data;
}

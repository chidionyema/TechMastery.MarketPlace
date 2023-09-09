// components/ForgotPassword.tsx
import { useState } from 'react';
import { requestPasswordReset } from '../api/auth';

function ForgotPassword() {
  const [email, setEmail] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await requestPasswordReset(email);
      // Display a message or redirect to a success page
    } catch (error) {
      console.error('Error requesting password reset:', error);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <input
        type="email"
        placeholder="Email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
      />
      <button type="submit">Request Reset</button>
    </form>
  );
}

export default ForgotPassword;

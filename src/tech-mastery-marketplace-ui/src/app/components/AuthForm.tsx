'use client';
import { useState, ChangeEvent, FormEvent } from 'react';
import { useRouter } from 'next/router';

interface AuthFormProps {
  title: string;
  onAuthenticate: (email: string, password: string) => void;
}

export default function AuthForm({ title, onAuthenticate }: AuthFormProps) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const router = useRouter();

  const handleFormSubmit = async (event: FormEvent) => {
    event.preventDefault();
    onAuthenticate(email, password);
  };

  return (
    <div>
      <h2>{title}</h2>
      <form onSubmit={handleFormSubmit}>
        <input
          type="email"
          placeholder="Email"
          value={email}
          onChange={(e: ChangeEvent<HTMLInputElement>) => setEmail(e.target.value)}
        />
        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e: ChangeEvent<HTMLInputElement>) => setPassword(e.target.value)}
        />
        <button type="submit">Submit</button>
      </form>
    </div>
  );
}

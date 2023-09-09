// hooks/useCurrentUser.ts

import { useEffect, useState } from 'react';
import { getCurrentUser } from '../api/auth'; // Implement your authentication API

export function useCurrentUser() {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  const logout = async () => {
    // Implement your logout logic here
  };

  useEffect(() => {
    const fetchCurrentUser = async () => {
      try {
        const currentUser = await getCurrentUser(); // Fetch the current user from your API
        setUser(currentUser);
      } catch (error) {
        setUser(null);
      } finally {
        setLoading(false);
      }
    };

    fetchCurrentUser();
  }, []);

  return { user, loading, logout };
}

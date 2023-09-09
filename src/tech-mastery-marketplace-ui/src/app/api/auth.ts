// api/auth.ts

import { Router } from 'express';
import { authenticateUser, registerUser } from '../services/auth'; // Implement authentication services

const router = Router();

router.post('/login', async (req, res) => {
  const { email, password } = req.body;
  try {
    const user = await authenticateUser(email, password);
    res.status(200).json(user);
  } catch (error) {
    res.status(401).json({ error: 'Invalid credentials' });
  }
});

router.post('/register', async (req, res) => {
  const { email, password, firstName, lastName } = req.body;
  try {
    const newUser = await registerUser(email, password, firstName, lastName);
    res.status(201).json(newUser);
  } catch (error) {
    res.status(400).json({ error: 'Registration failed' });
  }
});

export default router;

import AuthForm from '../components/AuthForm';

const LoginPage: React.FC = () => {
  const handleLogin = async (formData: { email: string; password: string }) => {
    // Implement your API call for authentication here
    // Return response or handle errors
    return {}; // Replace with actual response or error handling
  };

  return (
    <div className="flex flex-col items-center justify-center min-h-screen">
      <AuthForm title="Login" onSubmit={handleLogin} />
    </div>
  );
};

export default LoginPage;

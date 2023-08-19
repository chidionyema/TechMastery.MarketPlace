import AuthForm from '../components/AuthForm';

const RegisterPage: React.FC = () => {
  const handleRegister = async (formData: { email: string; password: string }) => {
    // Implement your API call for registration here
    // Return response or handle errors
    return {}; // Replace with actual response or error handling
  };

  return (
    <div className="flex flex-col items-center justify-center min-h-screen">
      <AuthForm title="Register" onSubmit={handleRegister} />
    </div>
  );
};

export default RegisterPage;

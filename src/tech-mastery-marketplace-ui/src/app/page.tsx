import Image from 'next/image';
import dynamic from 'next/dynamic';

const AuthForm = dynamic(() => import('./components/AuthForm'), { ssr: false });
const ProfilePage = dynamic(() => import('./components/ProfilePage'), { ssr: false });

export default function Home() {
  // ... Rest of your code ...

  return (
    <main className="flex min-h-screen flex-col items-center justify-between p-24">
      {/* Other content... */}

      {/* Conditional rendering based on login state */}
      {user.loggedIn ? (
        <ProfilePage user={user} />
      ) : (
        <AuthForm title="Login" onAuthenticate={handleAuthenticate} />
      )}

      {/* Other content... */}
    </main>
  );
}

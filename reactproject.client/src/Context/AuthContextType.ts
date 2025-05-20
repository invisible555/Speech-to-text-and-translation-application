interface AuthContextType {
     token: string | null;
  refreshToken: string | null;
  userLogin: string | null;
  role: string | null;
  login: (token: string, refreshToken: string, login: string, role: string) => void;
  logout: () => void;
}

export default AuthContextType
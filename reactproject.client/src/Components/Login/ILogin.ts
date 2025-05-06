interface ILogin {
    onSubmit: (data: { email: string; password: string }) => void;
}

export default ILogin
interface IRegister {
    onSubmit: (data: { email: string; login: string; password: string }) => void;
}

export default IRegister
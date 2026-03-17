function checkPasswordMatch(password, confirmPassword) {
    if (password !== confirmPassword) {
        return {
            ContainError: true,
            message: 'Passwords do not match. Please try again.'
        };
    }

    return {
        ContainError: false,
        message: ''
    };
}

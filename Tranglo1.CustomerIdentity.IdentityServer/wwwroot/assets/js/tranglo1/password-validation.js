
function validatePassword(password) {
    const errors = []

    if (password !== null && password !== undefined && password.length >= 1) {
        //does not contain lowercase letters
        if (!(password.match(/[a-z]/g))) {
            errors.push('1 lowercase letter')
        }

        //does not contain uppercase letters
        if (!(password.match(/[A-Z]/g))) {
            errors.push('1 capitalised letter')
        }

        //does not contain special character
        if (!(password.match(/[^a-zA-Z\d]/g))) {
            errors.push('At least 1 symbol (i.e !@#$%^&)')
        }

        //does not contain number
        if (!(password.match(/[0-9]/g))) {
            errors.push('1 number')
        }

        //does not contain more than 12 characters
        if (password.length < 12) {
            errors.push('Minimum 12 characters')
        }

        if (errors.length > 0) {
            return {
                ContainError: true,
                Errors: errors
            }
        } else {
            return {
                ContainError: false,
                Errors: null
            }
        }
    }
}

/* for peek password at login, sign up and reset password form */
function setupPeekPassword(input, icon) {
    const showPassword = () => { input.type = 'text'; setIconText(icon, 'visibility_off'); };
    const hidePassword = () => { input.type = 'password'; setIconText(icon, 'visibility'); };

    icon.addEventListener("mousedown", showPassword);
    icon.addEventListener("mouseup", hidePassword);
    icon.addEventListener("mouseleave", hidePassword);
}

function setIconText(icon, txt) {
    icon.textContent = '';
    void icon.offsetWidth;
    icon.textContent = txt;
}
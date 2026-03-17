let registryCodeValid = false;
let companyNameValid = false;
let fullNameValid = false;
let emailValid = false;
let passwordValid = false;
let countryValid = false;
let agreedToTermsValid = false;
let confirmPasswordValid = false;
let leadsOriginValid = false;
let otherLeadsOriginValid = false;

function setAllFieldAsInvalid() {
    registryCodeValid = false;
    companyNameValid = false;
    fullNameValid = false;
    countryValid = false;
    emailValid = false;
    passwordValid = false;
    agreedToTermsValid = false;
    confirmPasswordValid = false;
    leadsOriginValid = false;
    otherLeadsOriginValid = false;
}
function isFormValid() {
    if (selectedAccountType === 'business') {
        if (
            companyNameValid &&
            fullNameValid &&
            emailValid &&
            passwordValid &&
            countryValid &&
            agreedToTermsValid &&
            confirmPasswordValid &&
            leadsOriginValid &&
            otherLeadsOriginValid
        ) {
            enableCreateAccountButton();
        } else {
            disableCreateAccountButton();
        }
    } else if (selectedAccountType === 'individual') {
        if (
            companyNameValid &&
            emailValid &&
            passwordValid &&
            countryValid &&
            agreedToTermsValid &&
            confirmPasswordValid &&
            leadsOriginValid &&
            otherLeadsOriginValid
        ) {
            enableCreateAccountButton();
        } else {
            disableCreateAccountButton();
        }
    } else if (selectedAccountType === 'code') {
        if (signupCodeType === 'nonIndividual') {
            if (
                registryCodeValid &&
                fullNameValid &&
                emailValid &&
                passwordValid &&
                agreedToTermsValid && 
                confirmPasswordValid &&
                leadsOriginValid &&
                otherLeadsOriginValid
            ) {
                enableCreateAccountButton();
                return
            }
        } else if (signupCodeType === 'individual') {
            if (
                registryCodeValid &&
                emailValid &&
                passwordValid &&
                agreedToTermsValid &&
                confirmPasswordValid &&
                leadsOriginValid &&
                otherLeadsOriginValid
            ) {
                enableCreateAccountButton();
                return
            }
        }
            disableCreateAccountButton();
    } else {
        disableCreateAccountButton();
    }

}

function updateRegistryCodeValid(status) {
    registryCodeValid = status;
    isFormValid();
}

function displayFieldInvalid(feedbackElement, textField, dropdownField) {
    feedbackElement.classList.remove("text-success");
    feedbackElement.classList.add("text-danger");
    if (textField) {
        textField.foundation.setValid(false);
        textField.input.setCustomValidity("Invalid value");
    }
    if (dropdownField) {
        dropdownField.foundation.setValid(false);
    }
}

function displayFieldValid(feedbackElement, textField, dropdownField) {
    feedbackElement.classList.remove("text-danger");
    feedbackElement.classList.add("text-success");
    if (textField) {
        textField.foundation.setValid(true);
        textField.input.setCustomValidity("");
    }
    if (dropdownField) {
        dropdownField.foundation.setValid(true);
    }
}

function triggerFieldValidation() {
    $("#RegistryCode").trigger("input");
    $("#CompanyName").trigger("input");
    $("#FullName").trigger("input");
    $("#Email").trigger("input");
    $("#CountryISO2").trigger("change");
    $("#Password").trigger("input");
    $("#ConfirmPassword").trigger("input");
    $("#leadsOrigin").trigger("change");
    $("#OtherLeadsOrigin").trigger("input");
}

$('document').ready(function () {
    const textFields = [...document.querySelectorAll('.mdc-text-field')]
        .map(el => mdc.textField.MDCTextField.attachTo(el));

    const mdcSelects = [...document.querySelectorAll('.mdc-select')]
        .map(el => new mdc.select.MDCSelect(el));

    function removeSpecificErrorAlertMsg(fieldName) {
        const container = document.getElementById('ErrorAlert');
        if (!container || !container.innerHTML || container.innerHTML.trim().length === 0) return;

        // each error are split by <br> (or <br/>)
        const messages = container.innerHTML
            .split(/<br\s*\/?>/i)
            .map(m => m.trim())
            .filter(Boolean);

        const duplicateEmailMsgRegex = /^\s*Email\s+[^\s]+\s+already exists\.\s*$/i;
        const duplicateCompanyNameMsgRegex = /^\s*Company Name\s+already exists\..*$/i;
        const expiredRegistryCodeRegex = /^\s*This\s+Registry\s+Code\s+is\s+expired\.?\s*$/i;
        const invalidRegistryCodeRegex = /^\s*This\s+Invalid\s+Registry\s+Code\.?\s*$/i;

        let patterns = [];

        if (fieldName === 'all') {
            patterns.push(duplicateEmailMsgRegex, duplicateCompanyNameMsgRegex, expiredRegistryCodeRegex, invalidRegistryCodeRegex);
        } else if (fieldName === 'Email') {
            patterns.push(duplicateEmailMsgRegex);
        } else if (fieldName === 'CompanyName') {
            patterns.push(duplicateCompanyNameMsgRegex);
        } else if (fieldName === 'RegistryCode') {
            patterns.push(expiredRegistryCodeRegex, invalidRegistryCodeRegex);
        }

        // Remove any message that matches any of the given patterns
        const filtered = messages.filter(msg => {
            return !patterns.some(re => re.test(msg));
        });        
        container.innerHTML = filtered.length ? filtered.join('<br>') : ''; // Rebuild innerHTML
    }

    /** SIGN UP CODE FIELD*/
    const registryCodeElement = document.getElementById("RegistryCode");
    const registryCodeFeedbackElement = document.getElementById("RegistryCodeFeedback");
    const registryCodeWrapper = registryCodeElement.closest(".mdc-text-field");
    const registryCodeTextField = textFields.find(tf => tf.root === registryCodeWrapper);

    if (registryCodeFeedbackElement && registryCodeFeedbackElement.innerText.trim() !== "") {
        displayFieldInvalid(registryCodeElement, registryCodeTextField);
        registryCodeValid = false;
    }

    $('#RegistryCode').on('input', function () {
        var registryCode = $('#RegistryCode').val();
        if (registryCode.length <= 0) {
            displayFieldInvalid(registryCodeFeedbackElement, registryCodeTextField);
            registryCodeFeedbackElement.innerHTML = "Registry Code is required.";
            registryCodeValid = false;
        }
        else if ((registryCode.match(/^[A-Z0-9]+$/))) {
            displayFieldValid(registryCodeFeedbackElement, registryCodeTextField);
            registryCodeFeedbackElement.innerHTML = "Looks good.";
            registryCodeValid = true;
            removeSpecificErrorAlertMsg('RegistryCode');
        }
        else {
            displayFieldInvalid(registryCodeFeedbackElement, registryCodeTextField);
            registryCodeFeedbackElement.innerHTML = "Registry Code only allow alphanumeric with uppercase letter.";
            registryCodeValid = false;
        }
        isFormValid();
    });


    /** COMPANY NAME FIELD (if user didnt choose i have sign up code)*/
    const companyNameElement = document.getElementById("CompanyName");
    const companyNameFeedbackElement = document.getElementById("CompanyNameFeedback");
    const companyNameWrapper = companyNameElement.closest(".mdc-text-field");
    const companyNameTextField = textFields.find(tf => tf.root === companyNameWrapper);

    if (companyNameFeedbackElement && companyNameFeedbackElement.innerText.trim() !== "") {
        displayFieldInvalid(companyNameFeedbackElement, companyNameTextField);
        companyNameValid = false;
    }

    $('#CompanyName').on('input', function () {
        const companyNameValue = $('#CompanyName').val();
        if (companyNameValue.length == 0) {
            displayFieldInvalid(companyNameFeedbackElement, companyNameTextField);
            if (selectedAccountType == 'individual') {                
                companyNameFeedbackElement.innerHTML = "Preferred Name is required.";                
            } else {
                companyNameFeedbackElement.innerHTML = "Company Name is required.";
            }
            companyNameValid = false;
        } else if (companyNameValue.length > 150) {
            displayFieldInvalid(companyNameFeedbackElement, companyNameTextField);
            companyNameFeedbackElement.innerHTML = "Please enter 150 words or less.";
            companyNameValid = false;

        } else {
            displayFieldValid(companyNameFeedbackElement, companyNameTextField);
            companyNameFeedbackElement.innerHTML = "Looks good.";
            removeSpecificErrorAlertMsg('CompanyName');
            companyNameValid = true;
        }
        isFormValid();

    });

    /** FULL NAME FIELD (if user choose i have sign up code)*/
    const fullNameElement = document.getElementById("FullName");
    const fullNameFeedbackElement = document.getElementById("NameFeedback");
    const fullNameWrapper = fullNameElement.closest(".mdc-text-field");
    const fullNameTextField = textFields.find(tf => tf.root === fullNameWrapper);

    if (fullNameFeedbackElement && fullNameFeedbackElement.innerText.trim() !== "") {
        displayFieldInvalid(fullNameFeedbackElement, fullNameTextField);
        fullNameValid = false;
    }

    $('#FullName').on('input', function () {
        var name = $('#FullName').val();
        if (name.length <= 0) {
            displayFieldInvalid(fullNameFeedbackElement, fullNameTextField);
            fullNameFeedbackElement.innerHTML = "Name is required.";
            fullNameValid = false;
        }
        else if ((name.match(/^[\w\-\s]+$/))) {
            displayFieldValid(fullNameFeedbackElement, fullNameTextField);
            fullNameFeedbackElement.innerHTML = "Looks good.";
            fullNameValid = true;
        }
        else {
            displayFieldInvalid(fullNameFeedbackElement, fullNameTextField);
            fullNameFeedbackElement.innerHTML = "Name only allow alphanumeric.";
            fullNameValid = false;
        }

        isFormValid();

    });

    /** EMAIL FIELD */
    const emailElement = document.getElementById("Email");
    const emailFeedbackElement = document.getElementById("EmailFeedback");
    const emailWrapper = emailElement.closest(".mdc-text-field");
    const emailTextField = textFields.find(tf => tf.root === emailWrapper);

    if (emailFeedbackElement && emailFeedbackElement.innerText.trim() !== "") {
        displayFieldInvalid(emailFeedbackElement, emailTextField);
        emailValid = false;
    }

    $('#Email').on('input', function () {
        var email = $('#Email').val();
        if (email.length <= 0) {
            displayFieldInvalid(emailFeedbackElement, emailTextField);
            emailFeedbackElement.innerHTML = "Email is required.";
            emailValid = false;
        } else {
            if ((email.match(/^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/))) {
                displayFieldValid(emailFeedbackElement, emailTextField);
                emailFeedbackElement.innerHTML = "Looks good.";
                emailValid = true;
                removeSpecificErrorAlertMsg('Email');
            }
            else {
                displayFieldInvalid(emailFeedbackElement, emailTextField);
                emailFeedbackElement.innerHTML = "Please enter a valid email.";
                emailValid = false;
            }
        }
        isFormValid();
    });

    /** COUNTRY OF INCORPORATION FIELD */
    const countryFeedbackElement = document.getElementById("CountryFeedback");
    const countrySelectElement = document.getElementById("countrySelect");
    const countrySelectWrapper = countrySelectElement.closest(".mdc-select");
    const countryMDCSelect = mdcSelects.find(sel => sel.root === countrySelectWrapper);

    $('#CountryISO2').on('change', function () {
        let countryISO = $(this).val();
        if (countryISO.length > 1 && countryISO !== 'empty') {
            displayFieldValid(countryFeedbackElement, null, countryMDCSelect);
            countryFeedbackElement.innerHTML = "Looks good.";
            countryValid = true;
        } else {
            displayFieldInvalid(countryFeedbackElement, null, countryMDCSelect);
            countryFeedbackElement.innerHTML = "Please choose a country.";
            countryValid = false;
        }
        isFormValid();
    })

    /** PASSWORD & CONFIRM PASSWORD FIELD */
    const passwordElement = document.getElementById("Password");
    const passwordFeedbackElement = document.getElementById("passwordFeedback");
    const passwordWrapper = passwordElement.closest(".mdc-text-field");
    const passwordTextField = textFields.find(tf => tf.root === passwordWrapper);

    const confirmPasswordElement = document.getElementById("ConfirmPassword");
    const confirmPasswordFeedbackElement = document.getElementById("ConfirmPasswordFeedback");
    const confirmPasswordWrapper = confirmPasswordElement.closest(".mdc-text-field");
    const confirmPasswordTextField = textFields.find(tf => tf.root === confirmPasswordWrapper);

    $('#Password').on('input', function () {
        const passwordValue = $('#Password').val();
        const result = validatePassword(passwordValue);
        const errors = (result && result.Errors) || [];
        let passwordErrorString = errors.join(', ');

        if (passwordValue.length <= 0) {
            displayFieldInvalid(passwordFeedbackElement, passwordTextField);
            passwordFeedbackElement.innerHTML = "Password is required.";
            passwordValid = false;
        } else {
            if (result && result.ContainError == true) {
                displayFieldInvalid(passwordFeedbackElement, passwordTextField);
                passwordFeedbackElement.innerHTML = passwordErrorString + " required.";
                passwordValid = false;
            } else {
                displayFieldValid(passwordFeedbackElement, passwordTextField);
                passwordFeedbackElement.innerHTML = "All password requirements are met.";
                passwordValid = true;
            }
        }

        var confirmPasswordValue = $('#ConfirmPassword').val();
        if (confirmPasswordValue !== undefined) {
            if (confirmPasswordValue.length > 0 && passwordValue === confirmPasswordValue) {
                displayFieldValid(confirmPasswordFeedbackElement, confirmPasswordTextField);
                confirmPasswordFeedbackElement.innerHTML = "Looks good.";
                passwordValid = true;
                confirmPasswordValid = true;
            }

            if (confirmPasswordValue.length > 0 && passwordValue !== confirmPasswordValue) {
                displayFieldInvalid(confirmPasswordFeedbackElement, confirmPasswordTextField);
                confirmPasswordFeedbackElement.innerHTML = "Confirm Password does not match.";
                passwordValid = false;
                confirmPasswordValid = false;
            }
        }
        isFormValid();
    });

    $('#ConfirmPassword').on('input', function () {
        var confirmPasswordValue = $('#ConfirmPassword').val();
        if (confirmPasswordValue.length <= 0) {
            displayFieldInvalid(confirmPasswordFeedbackElement, confirmPasswordTextField);
            confirmPasswordFeedbackElement.innerHTML = "Confirm Password is required.";
            passwordValid = false;
            confirmPasswordValid = false;
        } else {
            const passwordToCompare = $('#Password').val();
            if (passwordToCompare === confirmPasswordValue) {
                displayFieldValid(confirmPasswordFeedbackElement, confirmPasswordTextField);
                confirmPasswordFeedbackElement.innerHTML = "Looks good.";
                passwordValid = true;
                confirmPasswordValid = true;
            }
            else {
                displayFieldInvalid(confirmPasswordFeedbackElement, confirmPasswordTextField);
                confirmPasswordFeedbackElement.innerHTML = "Confirm Password does not match.";
                passwordValid = false;
                confirmPasswordValid = false;
            }
        }
        isFormValid();
    });


    /** LEADS OF ORIGIN & SPECIFY OTHERS LEAD OF ORIGIN FIELD */
    const originFeedbackElement = document.getElementById("OriginFeedback");
    const originSelectElement = document.getElementById("leadsOriginSelect");
    const originSelectWrapper = originSelectElement.closest(".mdc-select");
    const originMDCSelect = mdcSelects.find(sel => sel.root === originSelectWrapper);

    const otherOriginElement = document.getElementById("OtherLeadsOrigin");
    const otherOriginFeedbackElement = document.getElementById("OtherLeadsOriginFeedback");
    const otherOriginWrapper = otherOriginElement.closest(".mdc-text-field");
    const otherOriginTextField = textFields.find(tf => tf.root === otherOriginWrapper);

    $('#leadsOrigin').on('change', function () {
        let leadsOrigin = $(this).val();

        if (leadsOrigin && leadsOrigin !== '0') {
            displayFieldValid(originFeedbackElement, null, originMDCSelect);
            originFeedbackElement.innerHTML = "Looks good.";
            leadsOriginValid = true;
        } else {
            displayFieldInvalid(originFeedbackElement, null, originMDCSelect);
            originFeedbackElement.innerHTML = "How did you find us is required.";
            leadsOriginValid = false;
        }

        if (leadsOrigin !== '10') {
            otherLeadsOriginValid = true;
        } else {
            otherLeadsOriginValid = false;
        }
        isFormValid();
    })

    $('#OtherLeadsOrigin').on('input', function () {
        otherLeadsOriginValid = false;
        const leadsOriginVal = document.getElementById("leadsOrigin").value;
        if (leadsOriginVal === '10') {
            let otherLeadsOrigin = $(this).val();
            if (otherLeadsOrigin.length > 0) {
                displayFieldValid(otherOriginFeedbackElement, otherOriginTextField);
                otherOriginFeedbackElement.innerHTML = "Looks good.";
                otherLeadsOriginValid = true;
            } else {
                displayFieldInvalid(otherOriginFeedbackElement, otherOriginTextField);
                otherOriginFeedbackElement.innerHTML = "This field is required.";
                otherLeadsOriginValid = false;
            }
        } else {
            otherOriginFeedbackElement.innerHTML = "";
            otherLeadsOriginValid = true;
        }
        isFormValid();
    })

    $('#IsTermsAndConditionReadAgreed').on('click', function () {
        const TOCfeedbackElement = document.getElementById("TOCfeedback");
        const isChecked = $('#IsTermsAndConditionReadAgreed')[0].checked
        if (isChecked == false) {
            TOCfeedbackElement.classList.remove("text-success");
            TOCfeedbackElement.classList.add('text-danger');
            TOCfeedbackElement.innerHTML = "You must agree to the Terms and Conditions!";
            agreedToTermsValid = false;
        }
        else {
            TOCfeedbackElement.innerHTML = "";
            agreedToTermsValid = true;
            triggerFieldValidation();
        }
        isFormValid();
    });

});

export default function EditLinks() {
    const divMessage = document.querySelector('#links-edit-container #message');
    const frmEdit = document.querySelector('#links-edit-container form');

    const txtTags = document.querySelector('#tags');

    if (frmEdit) {
        frmEdit.addEventListener('submit', (e) => onSubmitForm(e), false);
    }

    if (txtTags) {
        txtTags.addEventListener('blur', (e) => onTagsBlur(e), false);
    }

    const onSubmitForm = (e) => {
        e.preventDefault();
        e.stopPropagation();

        divMessage.classList.add('hide');
        divMessage.classList.remove('show');

        divMessage.classList.add('alert-success');
        divMessage.classList.remove('alert-danger');

        const validateResult = validateForm();

        if (!validateResult.isSuccess) {
            divMessage.innerHTML = validateResult.message;

            divMessage.classList.add('alert-danger');
            divMessage.classList.add('show');
        }
        else {
            submitUpdate();
        }
    }

    const onTagsBlur = (e) => {

    };

    const validateForm = () => {
        const requiredElements = Array.from(frmEdit.elements)
            .filter(element => element.hasAttribute('required') && element.tagName.toLowerCase() !== 'button');

        let hasErrors = false;
        let errorMsg = '';

        if (!requiredElements)
            return hasErrors;

        if (requiredElements) {
            requiredElements.forEach(element => {
                if (!element.checkValidity()) {
                    hasErrors = true;
                    element.classList.add('is-invalid');
                    errorMsg += `<li>${element.name} is required</li>`;
                }
                else {
                    element.classList.remove('is-invalid');
                }
            });
        };

        if (hasErrors) {
            errorMsg = `<ul>${errorMsg}</ul>`;
        }

        return { isSuccess: !hasErrors, message: errorMsg };
    }

    const submitUpdate = () => {
        const antiForgeInput = document.getElementsByName('__RequestVerificationToken')[0];
        const antiForgeToken = antiForgeInput ? antiForgeInput.value : null;

        const headers = new Headers();
        headers.append('Content-Type', 'application/json');
        headers.append('RequestVerificationToken', antiForgeToken);

        const request = {
            Id: frmEdit.id.value,
            Title: frmEdit.title.value,
            Description: frmEdit.description.value,
            Tags: frmEdit.tags ? frmEdit.tags.value.split(',') : []
        };

        //console.log(request);

        fetch('/links/edit', {
            headers: headers,
            method: 'POST',
            body: JSON.stringify(request),
        })
            .then(response => {
                if (response.ok) {
                    return response.json();
                } else {
                    throw new Error('Network response was not ok');
                }
            })
            .then(data => onEditSuccess(data))
            .catch(error => console.error('Error:', error));
    };

    const onEditSuccess = (data) => {
        const isSuccess = data.isSuccess ?? false;
        const message = data.message.trim() != '' ? data.message.trim() : "Link successfully saved";

        if(isSuccess) {
            divMessage.innerHTML = message;
            
            divMessage.classList.remove('hide');            
            divMessage.classList.remove('alert-danger');

            divMessage.classList.add('alert-success');
            divMessage.classList.add('show');
        }
        else {
            divMessage.innerHTML = message ?? "Unable to save the link";

            divMessage.classList.remove('show');
            divMessage.classList.remove('alert-success');

            divMessage.classList.add('alert-danger');
            divMessage.classList.add('show');
        };
    };
}
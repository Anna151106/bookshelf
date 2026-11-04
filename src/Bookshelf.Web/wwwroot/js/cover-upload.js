(() => {
    const allowedExtensions = ['jpg', 'jpeg', 'png', 'webp'];
    const maxSize = 5 * 1024 * 1024;

    const formatSize = bytes =>
        bytes < 1024 * 1024
            ? `${(bytes / 1024).toFixed(1)} КБ`
            : `${(bytes / (1024 * 1024)).toFixed(2)} МБ`;

    const validate = file => {
        const extension = file.name.includes('.') ? file.name.split('.').pop().toLowerCase() : '';
        if (!allowedExtensions.includes(extension)) {
            return 'Дозволені лише файли JPG, JPEG, PNG або WEBP.';
        }
        if (file.size > maxSize) {
            return 'Файл завеликий, максимум 5 МБ.';
        }
        return null;
    };

    document.querySelectorAll('[data-cover-upload]').forEach(root => {
        const input = root.querySelector('[data-cover-input]');
        const drop = root.querySelector('[data-cover-drop]');
        const preview = root.querySelector('[data-cover-preview]');
        const info = root.querySelector('[data-cover-info]');
        const details = root.querySelector('[data-cover-details]');
        const error = root.querySelector('[data-cover-error]');
        const originalSrc = preview.src;
        let objectUrl = null;

        const resetPreview = () => {
            if (objectUrl) {
                URL.revokeObjectURL(objectUrl);
                objectUrl = null;
            }
            preview.src = originalSrc;
            info.classList.add('d-none');
            details.textContent = '';
        };

        const clear = () => {
            input.value = '';
            resetPreview();
        };

        const accept = file => {
            const message = validate(file);
            if (message) {
                clear();
                error.textContent = message;
                return;
            }

            error.textContent = '';
            if (input.files[0] !== file) {
                const transfer = new DataTransfer();
                transfer.items.add(file);
                input.files = transfer.files;
            }

            resetPreview();
            objectUrl = URL.createObjectURL(file);
            preview.src = objectUrl;
            details.textContent = `${file.name} · ${formatSize(file.size)} · ${file.type || 'невідомий тип'}`;
            info.classList.remove('d-none');
        };

        root.querySelector('[data-cover-browse]').addEventListener('click', () => input.click());
        root.querySelector('[data-cover-clear]').addEventListener('click', () => {
            clear();
            error.textContent = '';
        });

        input.addEventListener('change', () => {
            if (input.files.length > 0) {
                accept(input.files[0]);
            }
        });

        ['dragenter', 'dragover'].forEach(type => drop.addEventListener(type, event => {
            event.preventDefault();
            drop.classList.add('is-dragover');
        }));

        ['dragleave', 'drop'].forEach(type => drop.addEventListener(type, event => {
            event.preventDefault();
            drop.classList.remove('is-dragover');
        }));

        drop.addEventListener('drop', event => {
            const file = event.dataTransfer.files[0];
            if (file) {
                accept(file);
            }
        });
    });
})();

/**
 * 
 * @returns The full api url, including version
 */
export const getApiUrl = () => {
    return `${process.env.API_URL}/v${process.env.API_VERSION}`;
}

/**
 * 
 * @returns The secret key to validate with the Api
 */
export const getApiKey = () => {
    return process.env.API_KEY;
}

/**
 * 
 * @returns Temporary user id. This is until the backend supports users.
 */
export const getUserId = () => {
    return process.env.USER_ID;
}